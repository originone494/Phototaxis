using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            BeAttackedSystem.Instance.onAttacked(playerControlSystem.Instance.sr.flipX);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (collision.CompareTag("Interactable"))
            {
                if (!playerControlSystem.Instance.isInteract)
                {
                    collision.GetComponent<InteractableObject>().OnTriggerByPlayer();
                }
            }
        }
    }
}