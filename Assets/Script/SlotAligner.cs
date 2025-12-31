using UnityEngine;

public class SlotAligner : MonoBehaviour
{
    public float spacing = 1.2f;

    void Start()
    {
        int count = transform.childCount;
        float startX = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Transform slot = transform.GetChild(i);
            slot.localPosition = new Vector3(startX + i * spacing, 0, 0);
        }
    }
}
