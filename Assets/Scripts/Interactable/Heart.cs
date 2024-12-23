using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerControlSystem.Instance.healthPoint < 3)
            {
                playerControlSystem.Instance.healthPoint++;
            }
            Destroy(gameObject);
            print("eat heart");
        }
    }
}