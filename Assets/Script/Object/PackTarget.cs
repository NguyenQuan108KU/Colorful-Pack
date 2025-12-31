using DG.Tweening;
using UnityEngine;

public class PackTarget : MonoBehaviour
{
    public ItemType packType;
    public Transform attachPoint;

    [Header("Slot Info")]
    public int slotIndex;

    [Header("Capacity")]
    public int capacity = 5;
    public int currentCount = 0;
    public bool isFull = false;

    public void AddItems(int count)
    {
        if (isFull) return;

        currentCount += count;

        if (currentCount >= capacity)
        {
            isFull = true;
            PackManager.instance.OnPackFilled(this);
        }
        else
        {
            Punch();
        }
    }

    void Punch()
    {
        transform.DOPunchScale(Vector3.one * 0.03f, 0.25f, 1, 0.7f);
    }

    public void FlyUp(System.Action onComplete)
    {
        transform.DOMoveY(transform.position.y + 6f, 0.8f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => onComplete?.Invoke());
    }
}
