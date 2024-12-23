using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NormalLight : MonoBehaviour
{
    public GameObject darkLightObject;
    public GameObject brightLightObject;

    private Light2D darkLight;
    private Light2D brightLight;

    public GameObject moth;

    [Header("是否有飞蛾")] public bool withMoth;

    [Header("昏暗区域灯光半径")] public float darkRadius;
    [Header("明亮区域灯光半径")] public float brightRadius;
    [Header("是否周期开启灯光")] public bool isLoop;
    [Header("灯光开启时间")] public float enableTime;
    [Header("灯光关闭时间")] public float closeTime;
    [Header("关闭前闪烁时间")] public float breathTime;
    [Header("灯光是否为开启状态")] public bool isRunning;

    [SerializeField] private float timeCounter;
    public Animator am;
    public bool isBreath;
    public bool isOpen;

    private void Start()
    {
        darkLight = darkLightObject.GetComponent<Light2D>();
        brightLight = brightLightObject.GetComponent<Light2D>();
        am = GetComponent<Animator>();

        isBreath = false;
        isOpen = true;
        timeCounter = 0;
        darkLight.pointLightOuterRadius = darkRadius;
        brightLight.pointLightOuterRadius = brightRadius;

        if (isRunning)
        {
            am.Play("Idle");
        }
        else
        {
            am.Play("Close");
        }

        if (withMoth)
        {
            am.Play("Close");
            moth.SetActive(true);
            isRunning = false;
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            if (isLoop)
            {
                timeCounter += Time.deltaTime;

                if (isOpen)
                {
                    if (timeCounter > enableTime)
                    {
                        isOpen = false;
                        isBreath = false;
                        timeCounter = 0;
                    }
                    else if (timeCounter > enableTime - breathTime)
                    {
                        isBreath = true;
                    }
                }
                else
                {
                    darkLight.intensity = 0;
                    brightLight.intensity = 0;
                    if (timeCounter > closeTime)
                    {
                        isOpen = true;
                        timeCounter = 0;
                    }
                }
            }

            if (isLoop)
                AnimationManager();
        }
        else
        {
            darkLight.intensity = 0;
            brightLight.intensity = 0;
        }
    }

    private void AnimationManager()
    {
        am.SetBool("isBreath", isBreath);
        am.SetBool("isOpen", isOpen);
    }

    private void BeingBrightAction()
    {
        withMoth = false;
        isRunning = true;
    }

    private void BeingDarkAction()
    {
        isRunning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position, darkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position, brightRadius);
    }
}