using System;
using System.Collections;
using UnityEngine;

public class talkWithObject : InteractableObject
{
    [Header("�Ի�����"), SerializeField]
    public string[] content;

    [Header("�Ի�ʱ���Ƿ��ܹ��ƶ�"), SerializeField] public bool canMove;

    [Header("�Ƿ��Զ�����")] public bool isAutoTrigggerDialog;

    [Header("������ɫ")] public Color color;

    public bool ishaveTriggered = false;//�Ƿ񴥷���

    public bool canInteract = false;

    private Coroutine coroutine;

    private void Start()
    {
        if (DialogSystem.Instance.HasDialogueOccurred(gameObject.name))
        {
            ishaveTriggered = true;
        }
    }

    //�ֶ�����
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

    //�Զ�����
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