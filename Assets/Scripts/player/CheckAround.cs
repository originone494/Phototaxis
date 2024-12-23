using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheckAround : MonoBehaviour
{
    #region 地面检测相关

    public Transform groundCheck;//用于检测是否接触地面
    public float collisionRadius = 0.02f;//检测是否在地面上使用的半径
    public float checkFallRadius = 0.1f;//检测地面，提前结束下落动画
    public LayerMask[] groundLayer;//地面层级

    #endregion 地面检测相关

    #region 墙相关

    public float checkWalloffset = 0.39f;//人物中心点到人物边缘的偏移
    public Transform headCheck;//头部顶端的位置

    #endregion 墙相关

    private void Update()
    {
        foreach (LayerMask t in groundLayer)
        {
            playerControlSystem.Instance.onGround = Physics2D.OverlapCircle(groundCheck.position, collisionRadius, t);
            if (playerControlSystem.Instance.onGround) break;
        }

        playerControlSystem.Instance.onRightWall = Physics2D.OverlapCircle(new Vector2(transform.position.x + checkWalloffset, transform.position.y), collisionRadius, groundLayer[0]);
        playerControlSystem.Instance.onLeftWall = Physics2D.OverlapCircle(new Vector2(transform.position.x - checkWalloffset, transform.position.y), collisionRadius, groundLayer[0]);
        playerControlSystem.Instance.onWall = playerControlSystem.Instance.onRightWall || playerControlSystem.Instance.onLeftWall ? true : false;
        playerControlSystem.Instance.wallSide = playerControlSystem.Instance.onRightWall ? -1 : 1;
    }
}