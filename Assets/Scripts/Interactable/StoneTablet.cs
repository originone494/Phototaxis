using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTablet : InteractableObject
{
    [Header("对话内容"), SerializeField] public string[] content;
    [Header("对话时，是否能够移动"), SerializeField] public bool canMove;
    [Header("是否自动触发")] public bool isAutoTrigggerDialog;
    [Header("是否能够重复触发")] public bool isTriggerAgain;
    [Header("字体颜色")] public Color color;

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