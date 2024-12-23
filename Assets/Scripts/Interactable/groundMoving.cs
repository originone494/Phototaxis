using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

public class groundMoving : MonoBehaviour
{
    private enum flatType
    {
        ONETIME, LINELOOP, CIRCLELOOP
    }

    [Header("类型"), SerializeField] private flatType platformType;

    [SerializeField, Header("起点、终点")] private Transform startPoint, endPoint;

    [Header("速度"), SerializeField] private float speed;

    private Transform targetPos;

    //ONETIME
    private bool isArrival = false;

    //CIRCLELOOP
    [SerializeField] private Transform center;

    [Header("是否是顺时针"), SerializeField] private bool isClockWise;

    private Quaternion r;

    [Header("是否运行")] public bool isRunning;

    private void Start()
    {
        transform.position = startPoint.position;
        targetPos = endPoint;
        r = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (isRunning)
        {
            move();
        }
        if (platformType == flatType.ONETIME)
        {
            if (isRunning)
            {
                if (!isArrival)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);
                }
                if (Vector2.Distance(transform.position, endPoint.position) < 0.1f)
                {
                    isArrival = true;
                }
            }
            else
            {
                if (isArrival)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPoint.position, speed * Time.deltaTime);
                }
                if (Vector2.Distance(transform.position, startPoint.position) < 0.1f)
                {
                    isArrival = false;
                }
            }
        }
    }

    private void move()
    {
        if (platformType == flatType.LINELOOP)
        {
            if (Vector2.Distance(transform.position, startPoint.position) < 0.1f)
            {
                targetPos = endPoint;
            }
            if (Vector2.Distance(transform.position, endPoint.position) < 0.1f)
            {
                targetPos = startPoint;
            }
            transform.position = Vector2.MoveTowards(transform.position, targetPos.position, speed * Time.deltaTime);
        }
        if (platformType == flatType.CIRCLELOOP)
        {
            transform.RotateAround(center.position, Vector3.forward, isClockWise ? -1 : 1 * Time.deltaTime * speed);
            transform.rotation = r;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}