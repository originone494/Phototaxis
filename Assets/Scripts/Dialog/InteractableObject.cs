using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public void OnTriggerByPlayer()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        print("base: interact with interactable object");
    }
}