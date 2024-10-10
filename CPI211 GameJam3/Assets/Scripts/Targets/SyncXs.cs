using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SyncXs : MonoBehaviour
{
    [SerializeField] GameObject otherLocation;
    [SerializeField] LilGuyAI lilGuy;
    [SerializeField] Camera currentCamera;
    [SerializeField] SyncXs nextSync;
    public bool canPlace;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && canPlace) {
            if (nextSync == null) { StartGame(); return; }
            canPlace = false; 
            nextSync.canPlace = true;
        }

        if (canPlace) { FollowMouse(); }
    }

    private void FollowMouse()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 25; // select distance = 10 units from the camera
        Debug.Log(currentCamera.ScreenToWorldPoint(mousePos));
        var mouseNow = currentCamera.ScreenToWorldPoint(mousePos);

        if (mouseNow.x < 10 && mouseNow.x > -10 && mouseNow.y < 16 && mouseNow.y > -4)
        {
            transform.position = currentCamera.ScreenToWorldPoint(mousePos);
            otherLocation.transform.localPosition = new Vector3(transform.localPosition.x * 3, 0.5f, transform.localPosition.y * 3);
        }
    }

    private void StartGame()
    {
        // fill in here
        lilGuy.canStart = true;
    }
}
