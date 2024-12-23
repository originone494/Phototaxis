using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pedal : MonoBehaviour
{
    [SerializeField, Header("是否一次性")] public bool isOneTime;

    [SerializeField, Header("恢复时间")] public float countTime;

    [SerializeField, Header("灯光")] public GameObject[] lights;

    private float timeCounter;
    private bool isPress;
    private Animator am;

    private void Start()
    {
        am = gameObject.GetComponent<Animator>();
        isPress = false;
        timeCounter = 0;
    }

    private void Update()
    {
        if (timeCounter > 0)
        {
            timeCounter -= Time.deltaTime;
        }
        else
        {
            if (!isOneTime && isPress)
            {
                foreach (var lt in lights)
                {
                    bool isLightRunning = false;

                    if (lt.GetComponent<NormalLight>())
                    {
                        NormalLight normalLight = lt.GetComponent<NormalLight>();
                        isLightRunning = normalLight.isRunning;
                    }
                    else if (lt.GetComponent<GravityLight>())
                    {
                        GravityLight gravityLight = lt.GetComponent<GravityLight>();
                        isLightRunning = gravityLight.isRunning;
                    }
                    else if (lt.GetComponent<ReverseLight>())
                    {
                        ReverseLight reverseLight = lt.GetComponent<ReverseLight>();
                        isLightRunning = reverseLight.isRunning;
                    }
                    else if (lt.GetComponent<TelePortLight>())
                    {
                        TelePortLight telePortLight = lt.GetComponent<TelePortLight>();
                        isLightRunning = telePortLight.isRunning;
                    }

                    if (isLightRunning)
                    {
                        Animator lightAm = lt.GetComponent<Animator>();

                        lightAm.Play("BeingDark");
                    }
                    else
                    {
                        Animator lightAm = lt.GetComponent<Animator>();
                        lightAm.Play("BeingBright");
                    }
                }

                isPress = false;
            }
        }
        am.SetBool("isPress", isPress);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPress)
        {
            if (collision.CompareTag("Player"))
            {
                timeCounter = countTime;
                isPress = true;

                foreach (var lt in lights)
                {
                    bool isLightRunning = false;

                    if (lt.GetComponent<NormalLight>())
                    {
                        NormalLight normalLight = lt.GetComponent<NormalLight>();
                        isLightRunning = normalLight.isRunning;
                    }
                    else if (lt.GetComponent<GravityLight>())
                    {
                        GravityLight gravityLight = lt.GetComponent<GravityLight>();
                        isLightRunning = gravityLight.isRunning;
                    }
                    else if (lt.GetComponent<ReverseLight>())
                    {
                        ReverseLight reverseLight = lt.GetComponent<ReverseLight>();
                        isLightRunning = reverseLight.isRunning;
                    }
                    else if (lt.GetComponent<TelePortLight>())
                    {
                        TelePortLight telePortLight = lt.GetComponent<TelePortLight>();
                        isLightRunning = telePortLight.isRunning;
                    }

                    if (isLightRunning)
                    {
                        Animator lightAm = lt.GetComponent<Animator>();

                        lightAm.Play("BeingDark");
                    }
                    else
                    {
                        Animator lightAm = lt.GetComponent<Animator>();
                        lightAm.Play("BeingBright");
                    }
                }
            }
        }
    }
}