using UnityEngine;

public class Slot : MonoBehaviour
{
    public Transform anchor;
    public DragItem currentItem;

    private void Awake()
    {
        EnsureCurrentItem();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep currentItem in sync while editing
        EnsureCurrentItem();
    }
#endif

    void EnsureCurrentItem()
    {
        if (currentItem == null)
        {
            // If a DragItem is already parented under this Slot (e.g. scene setup),
            // adopt it so IsEmpty() returns the correct value.
            currentItem = GetComponentInChildren<DragItem>();
            if (currentItem != null)
            {
                currentItem.transform.SetParent(transform);
            }
        }
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void SetItem(DragItem item)
    {
        currentItem = item;
    }
}
