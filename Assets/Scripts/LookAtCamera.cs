using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted,
    }

    [SerializeField] private Mode mode;
    [SerializeField] private bool3 freeze;
    private Vector3 startEulerAngles;

    private void Start()
    {
        startEulerAngles = transform.eulerAngles;
    }

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + directionFromCamera);
                break;
            case Mode.CameraForward:
                transform.LookAt(transform.position + Camera.main.transform.forward);
                break;
            case Mode.CameraForwardInverted:
                transform.LookAt(transform.position - Camera.main.transform.forward);
                break;
        }
        Vector3 eulerAngles = transform.eulerAngles;
        if (freeze.x)
        {
            eulerAngles.x = startEulerAngles.x;
        }
        if (freeze.y)
        {
            eulerAngles.y = startEulerAngles.y;
        }
        if (freeze.z)
        {
            eulerAngles.z = startEulerAngles.z;
        }
        transform.eulerAngles = eulerAngles;
    }
}
