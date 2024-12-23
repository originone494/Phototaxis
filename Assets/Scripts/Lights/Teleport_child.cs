using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport_child : MonoBehaviour
{
    public Transform tp_position;
    public GameObject parent;
    public TelePortLight tpl;

    private void Start()
    {
        tpl = parent.GetComponent<TelePortLight>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tpl.isRunning)
        {
            if (collision.CompareTag("Player") && parent.GetComponent<TelePortLight>().CanTeleport)
            {
                collision.transform.position = tp_position.position;
                parent.GetComponent<TelePortLight>().CanTeleport = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tpl.isRunning)
        {
            if (collision.CompareTag("Player"))
            {
                StopCoroutine(canNotTeleportForWhile(0));
                StartCoroutine(canNotTeleportForWhile(1f));
            }
        }
    }

    private IEnumerator canNotTeleportForWhile(float time)
    {
        yield return new WaitForSeconds(time);
        parent.GetComponent<TelePortLight>().CanTeleport = true;
    }
}