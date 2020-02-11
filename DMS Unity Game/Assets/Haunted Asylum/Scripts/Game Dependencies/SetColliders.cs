using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColliders : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject test = gameObject;
        for (int i = 0; i < test.transform.childCount; i++)
        {
            GameObject temp = test.transform.GetChild(i).gameObject;
            temp.AddComponent<MeshCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
