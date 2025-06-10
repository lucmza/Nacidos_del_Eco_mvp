using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
   
    
    public CinemachineVirtualCamera vcam;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (vcam == null) return;
       
        vcam.UpdateCameraState(Vector3.up, Time.deltaTime);
     
        var state = vcam.State;
        cam.transform.SetPositionAndRotation(state.FinalPosition, state.FinalOrientation);
    }
}
