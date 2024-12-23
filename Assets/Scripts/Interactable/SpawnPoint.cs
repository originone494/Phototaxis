using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : InteractableObject
{
    private Animator am;

    [Header("复活的位置")] public Transform rebirthPosition;

    private void Start()
    {
        am = GetComponent<Animator>();

        am.Play("Idle");
    }

    private void RebirthAnimationEvent()
    {
        //记录重生点
        playerControlSystem.Instance.rebirthPoint = rebirthPosition.position;
        playerControlSystem.Instance.rebirthGameObject = gameObject;

        //显示角色
        Color color = playerControlSystem.Instance.sr.color;
        playerControlSystem.Instance.sr.color = new Color(color.r, color.g, color.b, 1);

        am.Play("Idle");

        //复活后，角色可以控制自己
        playerControlSystem.Instance.canControl = true;

        //显示血条、体力条和对话模式黑块
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