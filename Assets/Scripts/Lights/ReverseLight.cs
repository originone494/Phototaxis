using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReverseLight : MonoBehaviour
{
    [Header("是否有飞蛾")] public bool withMoth;
    [Header("灯光是否运行")] public bool isRunning;

    public GameObject moth;

    private bool isEnter;
    private Collider2D target;
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
            am.Play("Die");
        }

        if (withMoth)
        {
            moth.SetActive(true);
            isRunning = false;
            am.Play("Die");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRunning)
        {
            StartCoroutine(StartReverseForWhile(1f));
            target = collision;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isRunning)
        {
            if (collision.CompareTag("Player"))
            {
                if (target == null)
                {
                    playerControlSystem.Instance.isReverseControl = true;
                    target = collision;
                }
                if (isEnter)
                {
                    playerControlSystem.Instance.isReverseControl = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isRunning)
        {
            if (collision.CompareTag("Player"))
            {
                StopCoroutine(StartReverseForWhile(0f));
                StartCoroutine(EndReverseForWhile(1f));
            }
        }
    }

    private IEnumerator StartReverseForWhile(float time)
    {
        yield return new WaitForSeconds(time);
        isEnter = true;
    }

    private IEnumerator EndReverseForWhile(float time)
    {
        yield return new WaitForSeconds(time);
        isEnter = false;
        playerControlSystem.Instance.isReverseControl = false;
        target = null;
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