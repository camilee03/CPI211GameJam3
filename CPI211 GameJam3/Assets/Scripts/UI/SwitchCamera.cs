using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] virtualCameras;
    int cameraNum;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { ChangeCameraLeft(); }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) { ChangeCameraRight(); }
    }

    private void ChangeCameraLeft()
    {
        virtualCameras[cameraNum].Priority = 0;

        if (cameraNum == virtualCameras.Length - 1) { 
            cameraNum = 0; 
        }
        else { cameraNum++; }

        virtualCameras[cameraNum].Priority = 10;
    }

    private void ChangeCameraRight()
    {
        virtualCameras[cameraNum].Priority = 0;
        if (cameraNum == 0) { 
            cameraNum = virtualCameras.Length - 1;
        }
        else { cameraNum--; }

        virtualCameras[cameraNum].Priority = 10;
    }
}
