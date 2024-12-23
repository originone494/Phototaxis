using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : InteractableObject
{
    private Animator am;

    [Header("�����λ��")] public Transform rebirthPosition;

    private void Start()
    {
        am = GetComponent<Animator>();

        am.Play("Idle");
    }

    private void RebirthAnimationEvent()
    {
        //��¼������
        playerControlSystem.Instance.rebirthPoint = rebirthPosition.position;
        playerControlSystem.Instance.rebirthGameObject = gameObject;

        //��ʾ��ɫ
        Color color = playerControlSystem.Instance.sr.color;
        playerControlSystem.Instance.sr.color = new Color(color.r, color.g, color.b, 1);

        am.Play("Idle");

        //����󣬽�ɫ���Կ����Լ�
        playerControlSystem.Instance.canControl = true;

        //��ʾѪ�����������ͶԻ�ģʽ�ڿ�
        UISystem.Instance.HpAndEnergy.SetActive(true);
        UISystem.Instance.DialogModeCanva.SetActive(true);

        playerControlSystem.Instance.isRebirthing = false;
    }

    protected override void Interact()
    {
        playerControlSystem.Instance.rebirthGameObject = gameObject;
        playerControlSystem.Instance.rebirthPoint = rebirthPosition.transform.position;

        if (LoadController.Instance != null)
        {
            LoadController.Instance.SaveBySerialization(rebirthPosition.position.x, rebirthPosition.position.y);
            print("saving success");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerControlSystem.Instance.isRebirthing && !playerControlSystem.Instance.isDead)
        {
            Interact();
        }
    }
}