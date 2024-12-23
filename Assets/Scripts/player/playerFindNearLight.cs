using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System.Runtime.CompilerServices;

public class playerFindNearLight : MonoBehaviour
{
    public GameObject[] closestObjects;
    public int lightPower;

    public string[] lightNames;

    [Header("是否显示灯光区域信息")] public bool isDisplayInfo;

    private void Update()
    {
        GetCurLightPower();
    }

    private void GetCurLightPower()
    {
        //初始光亮强度为0
        lightPower = 0;

        if (closestObjects != null)
        {
            closestObjects = new GameObject[] { };
        }

        //寻找最近的普通光
        GameObject[] nearLights = GameObject.FindGameObjectsWithTag("NormalLight");
        if (nearLights != null)
        {
            //寻找最近的两个正在运行的正常光
            if (nearLights.Length > 1)
            {
                var sortedObjects =
                nearLights.OrderBy
                (obj => Vector3.Distance(transform.position, obj.transform.position)).ToArray();

                NormalLight n1 = sortedObjects[0].GetComponent<NormalLight>();
                NormalLight n2 = sortedObjects[1].GetComponent<NormalLight>();

                bool isRun1 = n1.isRunning;
                if (n1.isLoop)
                {
                    isRun1 = n1.isOpen || n1.isBreath;
                }

                bool isRun2 = n2.isRunning;
                if (n2.isLoop)
                {
                    isRun2 = n1.isOpen || n1.isBreath;
                }

                if (isRun1 && isRun2)
                {
                    closestObjects = new GameObject[] { sortedObjects[0], sortedObjects[1] };
                }
                else if (isRun1 && !isRun2)
                {
                    closestObjects = new GameObject[] { sortedObjects[0] };
                }
                else if (!isRun1 && isRun2)
                {
                    closestObjects = new GameObject[] { sortedObjects[1] };
                }
            }
            else if (nearLights.Length == 1)
            {
                NormalLight n1 = nearLights[0].GetComponent<NormalLight>();

                bool isRun = n1.isRunning;
                if (n1.isLoop)
                {
                    isRun = n1.isOpen || n1.isBreath;
                }

                if (isRun)
                {
                    closestObjects = new GameObject[] { nearLights[0] };
                }
            }

            //如果找到的光的数量大于0
            if (closestObjects.Length > 0)
            {
                if (closestObjects.Length == 1)
                {
                    float distance = Vector2.Distance(closestObjects[0].transform.position, transform.position);

                    float darkRadius = closestObjects[0].GetComponent<NormalLight>().darkRadius;
                    float BrightRadius = closestObjects[0].GetComponent<NormalLight>().brightRadius;

                    if (distance < darkRadius && distance > BrightRadius)
                    {
                        lightPower++;
                    }
                    else if (distance < BrightRadius)
                    {
                        lightPower += 2;
                    }
                }
                else
                {
                    float distanceFirst = Vector2.Distance(closestObjects[0].transform.position, transform.position);
                    float distanceSecond = Vector2.Distance(closestObjects[1].transform.position, transform.position);

                    float darkRadiusFirst = closestObjects[0].GetComponent<NormalLight>().darkRadius;
                    float BrightRadiusFirst = closestObjects[0].GetComponent<NormalLight>().brightRadius;

                    float darkRadiusSecond = closestObjects[1].GetComponent<NormalLight>().darkRadius;
                    float BrightRadiusSecond = closestObjects[1].GetComponent<NormalLight>().brightRadius;

                    if (distanceFirst < darkRadiusFirst && distanceFirst > BrightRadiusFirst)
                    {
                        lightPower++;
                    }
                    else if (distanceFirst < BrightRadiusFirst)
                    {
                        lightPower += 2;
                    }

                    if (distanceSecond < darkRadiusSecond && distanceSecond > BrightRadiusSecond)
                    {
                        lightPower++;
                    }
                    else if (distanceSecond < BrightRadiusSecond)
                    {
                        lightPower += 2;
                    }
                }
            }
        }

        foreach (string name in lightNames)
        {
            if (lightPower >= 2) break;
            GameObject[] nearReverseLights = GameObject.FindGameObjectsWithTag(name);

            if (nearReverseLights != null)
            {
                foreach (GameObject near in nearReverseLights)
                {
                    Light2D nearLight = near.GetComponent<Light2D>();
                    if (nearLight != null)
                    {
                        if (near.CompareTag("GravityLight"))
                        {
                            GravityLight gravityLight = nearLight.GetComponent<GravityLight>();
                            if (gravityLight.isRunning)
                            {
                                if (Vector2.Distance(transform.position, nearLight.transform.position) < nearLight.pointLightOuterRadius)
                                {
                                    lightPower += 2; break;
                                }
                            }
                        }
                        else if (near.CompareTag("ReverseLight"))
                        {
                            ReverseLight reverseLight = nearLight.GetComponent<ReverseLight>();
                            if (reverseLight.isRunning)
                            {
                                if (Vector2.Distance(transform.position, nearLight.transform.position) < nearLight.pointLightOuterRadius)
                                {
                                    lightPower += 2; break;
                                }
                            }
                        }
                        else if (near.CompareTag("TeleportLight"))
                        {
                            TelePortLight telePortLight = nearLight.transform.parent.GetComponent<TelePortLight>();
                            if (telePortLight.isRunning)
                            {
                                if (Vector2.Distance(transform.position, nearLight.transform.position) < nearLight.pointLightOuterRadius)
                                {
                                    lightPower += 2; break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (lightPower == 1)
        {
            playerControlSystem.Instance.lightState = playerControlSystem.LightState.DIM;
            if (isDisplayInfo) print("暗淡");
        }
        else if (lightPower >= 2)
        {
            playerControlSystem.Instance.lightState = playerControlSystem.LightState.BRIGHT;
            if (isDisplayInfo) print("明亮");
        }
        else
        {
            playerControlSystem.Instance.lightState = playerControlSystem.LightState.DARK;
            if (isDisplayInfo) print("黑暗");
        }
    }
}