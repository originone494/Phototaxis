using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    public float shakeLevel = 3f;// 震动幅度
    public float setShakeTime = 0.5f;   // 震动时间
    public float shakeFps = 45f;    // 震动的FPS

    private bool isshakeCamera = false;// 震动标志
    private float fps;
    private float shakeTime = 0.0f;
    private float frameTime = 0.0f;
    private float shakeDelta = 0.005f;
    private Camera selfCamera;

    private void OnEnable()
    {
        isshakeCamera = true;
        selfCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        shakeTime = setShakeTime;
        fps = shakeFps;
        frameTime = 0.03f;
        shakeDelta = 0.005f;
    }

    private void OnDisable()
    {
        selfCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        isshakeCamera = false;
    }

    private void Update()
    {
        if (isshakeCamera)
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime <= 0)
                {
                    enabled = false;
                }
                else
                {
                    frameTime += Time.deltaTime;

                    if (frameTime > 1.0 / fps)
                    {
                        frameTime = 0;
                        selfCamera.rect = new Rect(shakeDelta * (-1.0f + shakeLevel * Random.value), shakeDelta * (-1.0f + shakeLevel * Random.value), 1.0f, 1.0f);
                    }
                }
            }
        }
    }
}