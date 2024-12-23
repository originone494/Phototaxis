using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GravityLight : MonoBehaviour
{
    [Header("�ƹ��Ƿ�����")] public bool isRunning;

    private Animator am;

    private void Start()
    {
        am = GetComponent<Animator>();

        if (isRunning)
        {
            am.Play("Idle");
        }
        else
        {
            am.Play("Close");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isRunning && !playerControlSystem.Instance.isTriggered) //�ƹ⿪�����ҽ�ɫ�սӴ�����������
        {
            //��ת��ɫ����������
            if (collision.CompareTag("Player") && playerControlSystem.Instance.isReverseGravity == false)
            {
                playerControlSystem.Instance.isReverseGravity = true;
                playerControlSystem.Instance.isTriggered = true;
            }
            else if (collision.CompareTag("Player") && playerControlSystem.Instance.isReverseGravity == true)
            {
                playerControlSystem.Instance.isReverseGravity = false;
                playerControlSystem.Instance.isTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isRunning)//��ɫ��������ʱ������isTriggered
        {
            if (collision.CompareTag("Player"))
            {
                playerControlSystem.Instance.isTriggered = false;
                //playerControlSystem.Instance.isReverseGravity = false;
            }
        }
    }

    private void BeingBrightEvent()
    {
        isRunning = true;
    }

    private void BeingDarkEvent()
    {
        isRunning = false;
    }
}