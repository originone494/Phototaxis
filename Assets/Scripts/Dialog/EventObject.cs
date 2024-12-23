using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class EventObject : MonoBehaviour
{
    [Header("�Ƿ��Զ�����")] public bool isAutoTrigggerDialog;

    [Header("�����ĶԻ�")] public EventBaseObject eventBaseObject;

    [Header("ͼ��")] public List<GameObject> images;
    [Header("����")] public List<GameObject> playables;

    public GameObject switchStage;

    private bool isHaveTrigger;

    private void Start()
    {
        switchStage.SetActive(false);
        isHaveTrigger = false;
    }

    private void Update()
    {
        if (EventSystem.Instance.isTeleport)
        {
            switchStage.SetActive(true);
        }
    }

    private void Interact()
    {
        EventSystem.Instance.Play(isAutoTrigggerDialog, eventBaseObject, images, playables);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !EventSystem.Instance.playerIsIntersect && !isHaveTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E) || isAutoTrigggerDialog)
            {
                playerControlSystem.Instance.rb.velocity = Vector2.zero;
                Interact();
                isHaveTrigger = true;
                EventSystem.Instance.playerIsIntersect = true;
                print("interset with eventdialog");
            }
        }
    }
}