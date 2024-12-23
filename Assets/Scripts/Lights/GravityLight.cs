using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GravityLight : MonoBehaviour
{
    [Header("灯光是否运行")] public bool isRunning;

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
        if (isRunning && !playerControlSystem.Instance.isTriggered) //灯光开启并且角色刚接触到此重力光
        {
            //反转角色的重力方向
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
        if (isRunning)//角色出重力光时，重置isTriggered
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