using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskTransform : MonoBehaviour
{
    public static DiskTransform instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
