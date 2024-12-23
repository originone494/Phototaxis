using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchHideEdge : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner2D confiner;

    [SerializeField] private Collider2D normalEdge;
    [SerializeField] private Collider2D hideEdge;

    private void Start()
    {
        confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = normalEdge;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = hideEdge;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        confiner.m_BoundingShape2D = normalEdge;
    }
}