using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    [Header("飞行速度")] public float shootSpped;
    [Header("销毁时间")] public float destoryTime;

    private Rigidbody2D rb;
    private SpriteRenderer sp;

    [SerializeField] private LayerMask layers;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = rb.GetComponent<SpriteRenderer>();
        Destroy(gameObject, destoryTime);

        //生成时，计算方向，飞向玩家
        Collider2D player = Physics2D.OverlapCircle((Vector2)transform.position, 10, LayerMask.GetMask("Player"));

        Vector2 dir = (player.transform.position - transform.position).normalized;

        rb.AddForce(dir * shootSpped);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == layers)
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Attack"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            BeAttackedSystem.Instance.PlayerBeAttackByWeb();

            Destroy(gameObject);
        }
    }
}