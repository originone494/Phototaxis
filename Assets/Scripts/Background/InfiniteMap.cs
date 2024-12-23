using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [Header("主摄像机")] public GameObject mainCamera;
    [Header("地图宽度")] public float mapWidth;
    [Header("地图重复的次数")] public int mapNums;

    private float m_totalWidth;//总地图宽度

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

        //获取当前位置
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