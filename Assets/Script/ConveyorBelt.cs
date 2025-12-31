using UnityEngine;
using System.Collections.Generic;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 2f;
    public float spacing = 1.5f;
    public float leftLimit = -8f;
    public int removeFirstCount = 4;   

    private List<Transform> items = new List<Transform>();
    private int removedCount = 0;
    private bool isStarted = false;
    public GameObject tutorial;
    void Start()
    {
        foreach (Transform child in transform)
        {
            items.Add(child);
        }

        SortItems();
    }

    void Update()
    {
        if (!isStarted)
        {
            if (Input.GetMouseButtonDown(0)) // PC + Mobile + Playable Ads
            {
                isStarted = true;
                tutorial.SetActive(false);
            }
            return;
        }
        for (int i = items.Count - 1; i >= 0; i--)
        {
            items[i].Translate(Vector3.left * speed * Time.deltaTime);
        }

        HandleItemAtLeft();
    }

    void HandleItemAtLeft()
    {
        if (items.Count == 0) return;

        Transform first = items[0];

        if (first.position.x < leftLimit)
        {
            if (removedCount < removeFirstCount)
            {
                items.RemoveAt(0);
                Destroy(first.gameObject);
                removedCount++;
            }
            else
            {
                Transform last = items[items.Count - 1];

                Vector3 newPos = last.position;
                newPos.x += spacing;

                first.position = newPos;

                items.RemoveAt(0);
                items.Add(first);
            }
        }
    }
    void SortItems()
    {
        items.Sort((a, b) => a.position.x.CompareTo(b.position.x));
    }
}
