using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pullRod : InteractableObject
{
    [SerializeField]
    public GameObject[] movingPlatForms;

    [SerializeField] private bool isOpen;
    [SerializeField] private bool canUse;

    private Animator am;

    private void Start()
    {
        am = GetComponent<Animator>();
        isOpen = false;
        canUse = true;
    }

    private void Update()
    {
        am.SetBool("isOpen", isOpen);
    }

    protected override void Interact()
    {
        if (!canUse) return;
        isOpen = isOpen ? false : true;
        canUse = false;

        foreach (var m in movingPlatForms)
        {
            m.GetComponent<groundMoving>().isRunning = m.GetComponent<groundMoving>().isRunning ? false : true;
        }
        Debug.Log("PullRood");
    }

    private void CanUseEvent()
    {
        canUse = true;
    }
}