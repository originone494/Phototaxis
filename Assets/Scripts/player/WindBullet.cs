using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBullet : MonoBehaviour
{
    [Header("·ÉÐÐËÙ¶È")] public float shootSpped;

    private Rigidbody2D rb;
    private SpriteRenderer sp;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = rb.GetComponent<SpriteRenderer>();
        if (playerControlSystem.Instance.sr.flipX)
        {
            sp.flipX = true;
            rb.velocity = -transform.right * shootSpped;
        }
        else
        {
            rb.velocity = transform.right * shootSpped;
        }
    }

    private void End()
    {
        Destroy(gameObject);
    }
}