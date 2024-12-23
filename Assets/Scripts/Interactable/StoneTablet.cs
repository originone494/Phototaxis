using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTablet : InteractableObject
{
    [Header("�Ի�����"), SerializeField] public string[] content;
    [Header("�Ի�ʱ���Ƿ��ܹ��ƶ�"), SerializeField] public bool canMove;
    [Header("�Ƿ��Զ�����")] public bool isAutoTrigggerDialog;
    [Header("�Ƿ��ܹ��ظ�����")] public bool isTriggerAgain;
    [Header("������ɫ")] public Color color;

    private bool isGet = false;
    private bool isTrigger = false;

    protected override void Interact()
    {
        if (!isGet)
        {
            playerControlSystem.Instance.StoneCount++;
            isGet = true;
            print("stone: " + playerControlSystem.Instance.StoneCount);
        }
        if (!isTrigger || isTriggerAgain)
        {
            DialogSystem.Instance.play(content, canMove, color, isAutoTrigggerDialog);
            isTrigger = true;
            print("stone dialog");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAutoTrigggerDialog && collision.CompareTag("Player"))
        {
            Interact();
        }
    }
}