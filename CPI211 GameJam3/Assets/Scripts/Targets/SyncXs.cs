using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncXs : MonoBehaviour
{
    [SerializeField] GameObject otherLocation;

    // Update is called once per frame
    void Update()
    {
        otherLocation.transform.localPosition = new Vector3(transform.localPosition.x* 5, 0.5f, transform.localPosition.y * 5);
    }
}
