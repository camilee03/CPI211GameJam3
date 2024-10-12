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
        if (Input.GetMouseButtonDown(0) && canPlace) {
            if (nextSync == null) { StartGame(); }
            else { nextSync.canPlace = true; }
            canPlace = false; 
        }

        if (canPlace) { FollowMouse(); }
    }

    private void FollowMouse()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 25; // select distance = 10 units from the camera
        var mouseNow = currentCamera.ScreenToWorldPoint(mousePos);
        Debug.Log(mouseNow);

        if (mouseNow.x < 10 && mouseNow.x > -10 && mouseNow.y < 30 && mouseNow.y > -10)
        {
            float magnitude = 14f;
            transform.position = currentCamera.ScreenToWorldPoint(mousePos);
            otherLocation.transform.localPosition = new Vector3(transform.localPosition.x * magnitude, -5f, (transform.localPosition.y + 10) * magnitude);
        }
    }

    private void StartGame()
    {
        // fill in here
        lilGuy.canStart = true;
    }
}
