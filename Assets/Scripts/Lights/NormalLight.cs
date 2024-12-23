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

    [Header("�Ƿ��зɶ�")] public bool withMoth;

    [Header("�谵����ƹ�뾶")] public float darkRadius;
    [Header("��������ƹ�뾶")] public float brightRadius;
    [Header("�Ƿ����ڿ����ƹ�")] public bool isLoop;
    [Header("�ƹ⿪��ʱ��")] public float enableTime;
    [Header("�ƹ�ر�ʱ��")] public float closeTime;
    [Header("�ر�ǰ��˸ʱ��")] public float breathTime;
    [Header("�ƹ��Ƿ�Ϊ����״̬")] public bool isRunning;

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