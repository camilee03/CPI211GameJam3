using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMap : MonoBehaviour
{
    [SerializeField] GameObject map;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) { map.SetActive(!map.activeSelf); }
    }
}
