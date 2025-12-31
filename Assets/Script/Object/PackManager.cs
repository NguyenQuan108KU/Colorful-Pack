using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class PackManager : MonoBehaviour
{
    public static PackManager instance;

    [Header("Pack Slots (scene)")]
    public Transform packRoot; // chứa 4 pack slot

    [Header("Pack Prefabs Queue")]
    public List<PackTarget> packPrefabs; // danh sách pack thay thế

    public List<PackTarget> activePacks = new List<PackTarget>();

    int prefabIndex = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Lấy pack sẵn trong scene
        activePacks.Clear();
        activePacks.AddRange(packRoot.GetComponentsInChildren<PackTarget>());

        // Gán slotIndex theo thứ tự
        for (int i = 0; i < activePacks.Count; i++)
        {
            activePacks[i].slotIndex = i;
        }
    }

    /// <summary>
    /// Tìm pack đang active đúng type
    /// </summary>
    public PackTarget GetPackInScene(ItemType type)
    {
        return activePacks.FirstOrDefault(p =>
            p.packType == type && !p.isFull
        );
    }

    /// <summary>
    /// Khi pack đầy
    /// </summary>
    public void OnPackFilled(PackTarget pack)
    {
        int slot = pack.slotIndex;
        Vector3 slotPos = pack.transform.position;

        pack.FlyUp(() =>
        {
            activePacks.Remove(pack);
            Destroy(pack.gameObject);

            SpawnNextPack(slot, slotPos);
        });
    }

    void SpawnNextPack(int slotIndex, Vector3 slotPos)
    {
        if (prefabIndex >= packPrefabs.Count)
            return;

        PackTarget newPack = Instantiate(
            packPrefabs[prefabIndex],
            slotPos + Vector3.up * 6f,
            Quaternion.identity,
            packRoot
        );

        prefabIndex++;

        newPack.slotIndex = slotIndex;
        activePacks.Add(newPack);

        newPack.transform.DOMoveY(slotPos.y, 0.6f)
            .SetEase(Ease.OutQuad);
    }
}
