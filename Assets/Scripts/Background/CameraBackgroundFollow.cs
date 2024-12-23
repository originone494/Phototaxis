using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundFollow : MonoBehaviour
{
    [Header("主相机的位置")] public Transform mainCamera;
    [Header("玩家的位置")] public Transform target;
    [Header("远层背景")] public Transform farBackground;
    [Header("中层背景")] public Transform middleBackground;
    [Header("近层背景")] public Transform nearBackground;

    private Vector2 m_lastPos;//最后一次相机的位置

    private void Start()
    {
        //记录相机的初始位置
        m_lastPos = transform.position;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        farBackground.position = new Vector3(mainCamera.position.x, mainCamera.position.y, 0f);
    }

    private void Update()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        //计算相机在上一帧和当前帧之间移动的距离
        Vector2 amountToMove = new Vector2(transform.position.x - m_lastPos.x, transform.position.y - m_lastPos.y);

        //根据相机移动的距离，移动中层背景和近层背景
        farBackground.position += new Vector3(amountToMove.x, amountToMove.y * 0.7f, 0f);
        middleBackground.position += new Vector3(amountToMove.x * 0.5f, amountToMove.y * 0.5f, 0f);
        nearBackground.position += new Vector3(amountToMove.x * 0.2f, amountToMove.y * 0.2f, 0f);

        //更新最后一次相机的位置
        m_lastPos = transform.position;
    }
}