using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip attack;
    public AudioClip doublejump;
    public AudioClip fly;
    public AudioClip jump;
    public AudioClip Run;
    public AudioClip walk;

    public AudioSource m_as;

    private void Start()
    {
        m_as = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerControlSystem.Instance.onGround && !playerControlSystem.Instance.isInteract
            && playerControlSystem.Instance.canControl)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_as.clip = jump;
                m_as.Play();
            }
        }
    }

    private void StopWalkAudipEvent()
    {
        if (m_as.clip == walk)
        {
            m_as.Pause();
        }
    }

    private void walkAudioEvent()
    {
        m_as.clip = walk;
        m_as.Play();
    }

    private void attackAudioEvent()
    {
        m_as.clip = attack;
        m_as.Play();
    }

    private void doubleJumpAudioEvent()
    {
        m_as.clip = doublejump;
        m_as.Play();
    }

    private void flyAudioEvent()
    {
        m_as.clip = fly;
        m_as.Play();
    }

    private void runAudioEvent()
    {
        m_as.clip = Run;
        m_as.Play();
    }
}