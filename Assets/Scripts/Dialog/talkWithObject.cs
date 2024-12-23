using System;
using System.Collections;
using UnityEngine;

public class talkWithObject : InteractableObject
{
    [Header("对话内容"), SerializeField]
    public string[] content;

    [Header("对话时，是否能够移动"), SerializeField] public bool canMove;

    [Header("是否自动触发")] public bool isAutoTrigggerDialog;

    [Header("字体颜色")] public Color color;

    public bool ishaveTriggered = false;//是否触发过

    public bool canInteract = false;

    private Coroutine coroutine;

    private void Start()
    {
        if (DialogSystem.Instance.HasDialogueOccurred(gameObject.name))
        {
            ishaveTriggered = true;
        }
    }

    //手动触发
    protected override void Interact()
    {
        if (!isAutoTrigggerDialog && !ishaveTriggered)
        {
            if (!canMove)
            {
                playerControlSystem.Instance.rb.velocity = Vector2.zero;
                playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.IDLE;
            }
            DialogSystem.Instance.Initial();
            DialogSystem.Instance.StartDialogue(gameObject.name);
            DialogSystem.Instance.play(content, canMove, color, isAutoTrigggerDialog);

            ishaveTriggered = true;
            print("talkWithObject by press E");
        }
    }

    //自动触发
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAutoTrigggerDialog && (!ishaveTriggered) && collision.CompareTag("Player")
            && !playerControlSystem.Instance.isRebirthing && !playerControlSystem.Instance.isDead)
        {
            ishaveTriggered = true;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(waitForWhile());
            print("talkWithObject auto trigger");
        }
    }

    private IEnumerator waitForWhile()
    {
        yield return new WaitForSeconds(0.1f);
        if (!canMove)
        {
            if (playerControlSystem.Instance.rb != null)
            {
                playerControlSystem.Instance.rb.velocity = Vector2.zero;
            }
            else
            {
                print("talkWihObject  rb is null");
            }

            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.IDLE;
        }
        DialogSystem.Instance.StartDialogue(gameObject.name);
        DialogSystem.Instance.play(content, canMove, color, isAutoTrigggerDialog);
    }
}