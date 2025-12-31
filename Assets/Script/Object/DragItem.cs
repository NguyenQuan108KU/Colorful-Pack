using UnityEngine;
using DG.Tweening;
public enum ItemType
{
    Yellow,
    Red,
    Blue,
    Green
}
public class DragItem : MonoBehaviour
{
    private static DragItem currentDrag;

    private Vector3 offset;
    private Vector3 startPos;
    private Transform startParent;
    private Camera cam;
    private Tween tween;
    private SpriteRenderer sr;
    private Vector3 startScale;
    private Slot startSlot;
    private Material mat;

    [Header("Outline")]
    public float focusOutlineSize = 1f;
    public float outlineTweenTime = 0.12f;
    private GameObject outline;
    public ItemType itemType;


    private void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
        outline = transform.GetChild(0).gameObject;
        outline.SetActive(false); // mặc định tắt
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPick();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.drag);
        }

        if (Input.GetMouseButton(0) && currentDrag == this)
        {
            Drag();
        }

        if (Input.GetMouseButtonUp(0) && currentDrag == this)
        {
            Drop();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.drog);
        }
    }

    void TryPick()
    {
        TrayManager.instance.NotifyUserInteraction();
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorld);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                currentDrag = this;

                tween?.Kill();
                startPos = transform.position;
                startParent = transform.parent;

                startSlot = startParent ? startParent.GetComponent<Slot>() : null;

                offset = transform.position - (Vector3)mouseWorld;
                sr.sortingOrder = 10;
                outline.GetComponent<SpriteRenderer>().sortingOrder = 9;
                ShowOutline(true);
                return;
            }
        }
    }


    void Drag()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + offset;
    }

    void Drop()
    {
        sr.sortingOrder = 0;
        TrayManager.instance.ResetTimer();
        outline.GetComponent<SpriteRenderer>().sortingOrder = -1;
        currentDrag = null;
        ShowOutline(false);
        PlayDropScale();

        Slot slot = FindNearestSlot();

        if (slot != null && (slot.IsEmpty() || slot.transform == startParent))
            Snap(slot);
        else
            Return();
    }


    void Snap(Slot slot)
    {
        Slot oldSlot = startParent ? startParent.GetComponent<Slot>() : null;
        if (oldSlot != null)
            oldSlot.SetItem(null);

        transform.SetParent(slot.transform);

        tween = transform.DOLocalMove(
                    slot.anchor.localPosition,
                    0.25f
                )
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    slot.SetItem(this);

                    Tray tray = slot.GetComponentInParent<Tray>();
                    if (tray != null)
                        tray.CheckMatch();
                });
    }

    void Return()
    {
        if (startSlot == null || startSlot.anchor == null)
            return;

        transform.SetParent(startSlot.transform);

        tween = transform.DOLocalMove(
                    startSlot.anchor.localPosition,
                    0.5f
                )
                .SetEase(Ease.OutQuad);
    }

    Slot FindNearestSlot()
    {
        Slot[] slots = FindObjectsOfType<Slot>();
        float min = 1f;
        Slot best = null;

        foreach (var s in slots)
        {
            if (s.anchor == null) continue;

            // Skip occupied slots except the one we started from
            if (!s.IsEmpty() && s.transform != startParent) continue;

            float d = Vector2.Distance(transform.position, s.anchor.position);
            if (d < min)
            {
                min = d;
                best = s;
            }
        }
        return best;
    }

    void PlayDropScale()
    {
        tween?.Kill();

        transform.localScale = startScale;

        transform.DOScale(startScale * 0.85f, 0.1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(startScale, 0.12f)
                    .SetEase(Ease.OutBack);
            });
    }
    void ShowOutline(bool show)
    {
        if (outline != null)
            outline.SetActive(show);
    }

}
