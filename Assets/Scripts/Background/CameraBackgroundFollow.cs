using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundFollow : MonoBehaviour
{
    [Header("�������λ��")] public Transform mainCamera;
    [Header("��ҵ�λ��")] public Transform target;
    [Header("Զ�㱳��")] public Transform farBackground;
    [Header("�в㱳��")] public Transform middleBackground;
    [Header("���㱳��")] public Transform nearBackground;

    private Vector2 m_lastPos;//���һ�������λ��

    private void Start()
    {
        //��¼����ĳ�ʼλ��
        m_lastPos = transform.position;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        farBackground.position = new Vector3(mainCamera.position.x, mainCamera.position.y, 0f);
    }

    private void Update()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        //�����������һ֡�͵�ǰ֮֡���ƶ��ľ���
        Vector2 amountToMove = new Vector2(transform.position.x - m_lastPos.x, transform.position.y - m_lastPos.y);

        //��������ƶ��ľ��룬�ƶ��в㱳���ͽ��㱳��
        farBackground.position += new Vector3(amountToMove.x, amountToMove.y * 0.7f, 0f);
        middleBackground.position += new Vector3(amountToMove.x * 0.5f, amountToMove.y * 0.5f, 0f);
        nearBackground.position += new Vector3(amountToMove.x * 0.2f, amountToMove.y * 0.2f, 0f);

        //�������һ�������λ��
        m_lastPos = transform.position;
    }
}