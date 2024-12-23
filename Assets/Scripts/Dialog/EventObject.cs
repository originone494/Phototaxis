using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class EventObject : MonoBehaviour
{
    [Header("是否自动触发")] public bool isAutoTrigggerDialog;

    [Header("触发的对话")] public EventBaseObject eventBaseObject;

    [Header("图像")] public List<GameObject> images;
    [Header("动画")] public List<GameObject> playables;

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