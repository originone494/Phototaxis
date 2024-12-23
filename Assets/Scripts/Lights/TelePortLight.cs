using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelePortLight : MonoBehaviour
{
    [Header("灯光是否运行")] public bool isRunning;

    public bool CanTeleport;

    [HideInInspector] public Animator am;

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

    private void BeingBrightAction()
    {
        isRunning = true;
    }

    private void BeingDarkAction()
    {
        isRunning = false;
    }
}