using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwichCameraFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;

    [SerializeField, Header("角色在左侧")] private Transform followAtLeft;
    [SerializeField, Header("角色在右侧")] private Transform followAtRight;
    [SerializeField, Header("角色在中间")] private Transform followAtCenter;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 5)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            virtualCamera.Follow = followAtCenter;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (SceneManager.GetActiveScene().buildIndex != 5)
        {
            if (collision.CompareTag("followAtLeft"))
            {
                if (followAtLeft != null)
                {
                    virtualCamera.Follow = followAtLeft;
                }
            }
            else if (collision.CompareTag("followAtRight"))
            {
                if (followAtRight != null)
                {
                    virtualCamera.Follow = followAtRight;
                }
            }
            else if (collision.CompareTag("followAtCenter"))
            {
                if (followAtCenter != null)
                {
                    virtualCamera.Follow = followAtCenter;
                }
            }
        }
    }
}