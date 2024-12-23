using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTeleport : MonoBehaviour
{
    public GameObject switchStage;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switchStage.GetComponent<SwitchStage>().WhiteToBlack();
        }
    }
}