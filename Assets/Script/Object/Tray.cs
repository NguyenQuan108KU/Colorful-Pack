using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;

public class Tray : MonoBehaviour
{
    public float spacing = 0.2f;      // khoảng cách giữa item
    public float moveTime = 0.2f;
    public GameObject diskPrefab;
    public Transform diskTransform;
    public float shrinkTime = 0.1f;
    public float itemToDiskTime = 0.1f;
    public float attachDelay = 0.15f;   // item chậm theo disk
    public float followSmooth = 0.25f;  // độ mượt
    public bool isCompleted = false;
    public Slot[] slots;
    private void Start()
    {
        slots = GetComponentsInChildren<Slot>();
    }
    // Made very small so items become extremely tiny when attached to disk.
    public Vector3 diskItemScale = new Vector3(0.03f, 0.03f, 0.03f);
    public void CheckMatch()
    {
        DragItem[] items = GetComponentsInChildren<DragItem>();

        var groups = items.GroupBy(i =>
        {
            var sr = i.GetComponent<SpriteRenderer>();
            return sr != null && sr.sprite != null
                ? sr.sprite.name
                : i.gameObject.name;
        });

        foreach (var g in groups)
        {
            if (g.Count() < 5) continue;
            isCompleted = true;
            var matchedItems = g.Take(5).ToList();
            ItemType type = matchedItems[0].itemType;

            PackTarget targetPack =
    PackManager.instance.GetPackInScene(type);

            if (targetPack != null)
            {
                MoveToPackLikeDisk(matchedItems, targetPack);
            }
            else
            {
                MoveToCenter(matchedItems);
            }


            return; // chỉ xử lý 1 match mỗi lần
        }
    }


    bool IsValidForTween(DragItem item)
    {
        return item != null && item.gameObject != null && item.transform != null;
    }

    void MoveToPackLikeDisk(List<DragItem> items, PackTarget pack)
    {
        if (items == null || items.Count < 3) return;
        if (pack == null || pack.attachPoint == null) return;

        Transform packAttach = pack.attachPoint;

        // sort order
        for (int i = 0; i < items.Count; i++)
        {
            var sr = items[i].GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = i;
        }

        // 1️⃣ chọn tâm
        DragItem center = items[2];
        Vector3 centerPos = center.transform.position;

        float startX = -(items.Count - 1) * spacing * 0.5f;
        Sequence gatherSeq = DOTween.Sequence();

        // 2️⃣ chụm item
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (!IsValidForTween(item)) continue;

            Vector3 target =
                centerPos + new Vector3(startX + i * spacing, 0, 0);

            gatherSeq.Join(
                item.transform.DOMove(target, moveTime)
                    .SetEase(Ease.OutBack)
                    .SetLink(item.gameObject)
            );
        }

        // 3️⃣ bay vào pack
        gatherSeq.OnComplete(() =>
        {
            float spacingRatio = 0.3f;
            Vector3 targetScale = Vector3.one * 0.35f;

            Sequence master = DOTween.Sequence();

            foreach (var item in items)
            {
                if (!IsValidForTween(item)) continue;

                item.transform.DOKill();

                Vector3 offset = item.transform.position - centerPos;
                Vector3 targetPos = packAttach.position + offset * spacingRatio;

                Sequence seq = DOTween.Sequence();

                seq.Join(
                    item.transform.DOJump(
                        targetPos,
                        1f,
                        1,
                        itemToDiskTime
                    ).SetEase(Ease.InOutQuad)
                );

                seq.Join(
                    item.transform.DOScale(targetScale, itemToDiskTime)
                );

                seq.OnComplete(() =>
                {
                    if (item != null)
                        Destroy(item.gameObject);
                });

                master.Join(seq);
            }

            // 4️⃣ báo pack đã nhận item
            master.OnComplete(() =>
            {
                pack.AddItems(items.Count); // hoặc pack.OnItemsArrived(...)
                Disappear();
            });
        });
    }

    void MoveToCenter(List<DragItem> items)
    {
        if (items == null || items.Count < 3) return;

        DragItem center = items[2];
        if (!IsValidForTween(center)) return;

        Vector3 centerPos = center.transform.position;

        float startX = -(items.Count - 1) * spacing * 0.5f;

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < items.Count; i++)
        {
            var itm = items[i];
            if (!IsValidForTween(itm)) continue;

            var sr = itm.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = i;

            Vector3 target =
                centerPos + new Vector3(startX + i * spacing, 0, 0);

            // capture the target position and create a move tween that uses the item's transform directly.
            // We SetLink immediately to the item's GameObject to avoid DOTween trying to operate on a destroyed transform.
            var moveTween = itm.transform.DOMove(target, moveTime)
                .SetEase(Ease.OutBack)
                .SetLink(itm.gameObject);

            seq.Join(moveTween);
        }


        seq.OnComplete(() =>
        {

            foreach (var item in items)
            {
                if (item == null) continue;
                item.transform.SetParent(null, true);

            }

            SpawnDisk(items, centerPos);
        });

    }

    void SpawnDisk(List<DragItem> items, Vector3 centerPos)
    {
        // safe access to DiskTransform singleton; fallback to centerPos if not available
        Vector3 spawnPos = DiskTransform.instance != null && DiskTransform.instance.transform != null
            ? DiskTransform.instance.transform.position
            : centerPos;

        GameObject diskObj = Instantiate(
            diskPrefab,
            spawnPos,
            Quaternion.identity
        );

        Disk disk = diskObj != null ? diskObj.GetComponent<Disk>() : null;

        float spacingRatio = 0.6f;
        float jumpPower = 1.0f;          // độ cao nhảy
        int jumpCount = 1;               // 1 lần nhảy (bay lên rồi rơi)
        Vector3 targetScale = new Vector3(0.35f, 0.35f, 0.35f);

        // master sequence that waits for all item-to-disk sequences
        Sequence master = DOTween.Sequence();

        foreach (var item in items)
        {
            if (!IsValidForTween(item)) continue;

            var it = item;

            it.transform.DOKill();

            // capture start and target positions immediately
            Vector3 startPos = it.transform.position;
            Vector3 offset = startPos - centerPos;
            Vector3 compressedOffset = offset * spacingRatio;

            Vector3 diskPos = (diskObj != null && diskObj.transform != null) ? diskObj.transform.position : spawnPos;
            Vector3 targetPos = diskPos + compressedOffset;

            Sequence seq = DOTween.Sequence();

            var jumpTween = it.transform.DOJump(
                    targetPos,
                    jumpPower,
                    jumpCount,
                    itemToDiskTime
                ).SetEase(Ease.InOutQuad);

            var scaleTween = it.transform.DOScale(targetScale, itemToDiskTime)
                    .SetEase(Ease.OutQuad);

            // Link both tweens / sequence to the item so they get killed if item is destroyed.
            jumpTween.SetLink(it.gameObject);
            scaleTween.SetLink(it.gameObject);
            seq.SetLink(it.gameObject);

            seq.Join(jumpTween);
            seq.Join(scaleTween);

            seq.AppendInterval(attachDelay);

            seq.AppendCallback(() =>
            {
                if (it == null) return;
                if (disk != null && disk.transform != null)
                {
                    it.transform.SetParent(disk.transform, true);
                }
                it.transform.localScale = targetScale;
                if (disk != null)
                {
                    disk.AddItem(it.transform);
                }
            });

            // join child into master
            master.Join(seq);
        }

        // disappear only after all item sequences complete
        master.OnComplete(() =>
        {
            Disappear();
        });
    }
    public void Disappear()
    {
        transform.DOKill();

        TrayManager manager = GetComponentInParent<TrayManager>();

        // 1️⃣ GỌI DỒN XUỐNG NGAY (TẠO GAP)
        if (manager != null)
        {
            manager.CompleteTray(transform);
        }
        Sequence seq = DOTween.Sequence();
        seq.Append(
            transform.DOScale(Vector3.zero, 0.18f)
                .SetEase(Ease.InOutCubic)
        );

        seq.SetLink(gameObject);

        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    public Slot GetEmptySlot()
    {
        if (isCompleted) return null;

        foreach (Slot slot in slots)
        {
            if (slot.IsEmpty())
                return slot;
        }
        return null;
    }
    public string GetMainItemKey()
    {
        DragItem[] items = GetComponentsInChildren<DragItem>();
        if (items.Length == 0) return null;

        return items
            .GroupBy(i =>
            {
                var sr = i.GetComponent<SpriteRenderer>();
                return sr != null && sr.sprite != null
                    ? sr.sprite.name
                    : i.gameObject.name;
            })
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }
}
