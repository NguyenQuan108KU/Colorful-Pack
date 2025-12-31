using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAligner : MonoBehaviour
{
    public float spacing = 1.2f;

    void Start()
    {
        int count = transform.childCount;
        float startY = (count - 1) * spacing * 0.5f;
        for (int i = 0; i < count; i++)
        {
            Transform slot = transform.GetChild(i);
            slot.localPosition = new Vector3(0, startY - i * spacing, 0);
        }
    }
}
