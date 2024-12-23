using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraObject : MonoBehaviour
{
    public ShakeCamera shakeCamera;

    public void OnEnable()
    {
        shakeCamera = GetComponent<ShakeCamera>();
        shakeCamera.enabled = true;
    }

    public void OnDisable()
    {
        shakeCamera.enabled = false;
    }
}