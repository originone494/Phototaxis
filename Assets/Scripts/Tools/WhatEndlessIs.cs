using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhatEndlessIs : MonoBehaviour
{
    public GameObject TrueEnd;
    public GameObject BadEnd;

    private void Start()
    {
        if (playerControlSystem.Instance.StoneCount == 3)
        {
            TrueEnd.SetActive(true);
        }
        else
        {
            BadEnd.SetActive(true);
        }
    }
}