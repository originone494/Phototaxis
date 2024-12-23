using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerAnimator : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    private CheckAround checkAround;
    private Movement movement;
    private Rigidbody2D rb;

    public int side = 1;//人物的转向

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        checkAround = GetComponent<CheckAround>();
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        turnFaceDirection();

        SwitchAnim();
    }

    private void turnFaceDirection()
    {
        side = sr.flipX ? -1 : 1;

        if (!playerControlSystem.Instance.onGround && playerControlSystem.Instance.onWall)
        {
            if (side != playerControlSystem.Instance.wallSide)
            {
                side *= -1;
                Flip();
            }
        }
    }

    public void Flip()
    {
        if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        sr.flipX = (side == 1) ? false : true;
    }

    private void setAllFalse()
    {
        anim.SetBool("walk", false);
        anim.SetBool("run", false);
        anim.SetBool("fall", false);
        anim.SetBool("jump", false);
        anim.SetBool("doubleJump", false);
        anim.SetBool("fly", false);
        anim.SetBool("climb", false);
        anim.SetBool("attack", false);
        anim.SetBool("web", false);
    }

    private void SwitchAnim()
    {
        switch (playerControlSystem.Instance.animationState)
        {
            case playerControlSystem.AnimationState.WEB:
                setAllFalse();
                anim.SetBool("web", true);
                break;

            case playerControlSystem.AnimationState.RUN:
                setAllFalse();
                anim.SetBool("run", true);
                break;
            //二段跳的动画优先于普通跳
            case playerControlSystem.AnimationState.DOUBLEJUMP:
                setAllFalse();
                anim.SetBool("doubleJump", true);
                break;

            case playerControlSystem.AnimationState.JUMP:
                setAllFalse();
                anim.SetBool("jump", true);
                break;
            //飞的动画优先于坠落
            case playerControlSystem.AnimationState.FLY:
                setAllFalse();
                anim.SetBool("fly", true);
                break;
            //爬的动画优先于坠落
            case playerControlSystem.AnimationState.CLIMB:
                setAllFalse();
                anim.SetBool("climb", true);
                break;

            case playerControlSystem.AnimationState.FALL:
                setAllFalse();
                anim.SetBool("fall", true);
                break;

            //walk动画优先级最低
            case playerControlSystem.AnimationState.WALK:
                setAllFalse();
                anim.SetBool("walk", true);
                break;

            case playerControlSystem.AnimationState.ATTACK:
                setAllFalse();
                anim.SetBool("attack", true);
                break;

            default:
                setAllFalse();
                break;
        }
    }
}