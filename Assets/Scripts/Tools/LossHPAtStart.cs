using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossHPAtStart : MonoBehaviour
{
    private bool isTrigger = false;

    private void Update()
    {
        if (!isTrigger)
        {
            if (playerControlSystem.Instance != null && playerControlSystem.Instance.canControl)
            {
                playerControlSystem.Instance.healthPoint = 2;
                isTrigger = true;
            }
        }
    }
}