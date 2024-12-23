using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [Header("�������")] public GameObject mainCamera;
    [Header("��ͼ���")] public float mapWidth;
    [Header("��ͼ�ظ��Ĵ���")] public int mapNums;

    private float m_totalWidth;//�ܵ�ͼ���

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        m_totalWidth = mapWidth * mapNums;
    }

    private void Update()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        m_totalWidth = mapWidth * mapNums;

        //��ȡ��ǰλ��
        Vector3 tempPosition = transform.position;
        if (mainCamera.transform.position.x > transform.position.x + m_totalWidth / 2)
        {
            tempPosition.x += m_totalWidth;
            transform.position = tempPosition;
        }
        else if (mainCamera.transform.position.x < transform.position.x - m_totalWidth / 2)
        {
            tempPosition.x -= m_totalWidth;
            transform.position = tempPosition;
        }
    }
}