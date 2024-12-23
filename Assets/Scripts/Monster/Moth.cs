using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moth : MonoBehaviour
{
    public Animator am;
    public bool isFlyAway;

    private GameObject parent;

    private void Start()
    {
        am = GetComponent<Animator>();
        am.Play("idle");
        isFlyAway = false;

        parent = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFlyAway)
        {
            if (collision.CompareTag("Attack"))
            {
                Animator lightAm = parent.GetComponent<Animator>();
                lightAm.Play("BeingBright");

                isFlyAway = true;
                am.SetBool("isFly", true);
            }
        }
    }

    private void AnimatorFly()
    {
        gameObject.SetActive(false);
    }
}