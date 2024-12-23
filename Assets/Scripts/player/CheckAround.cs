using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheckAround : MonoBehaviour
{
    #region ���������

    public Transform groundCheck;//���ڼ���Ƿ�Ӵ�����
    public float collisionRadius = 0.02f;//����Ƿ��ڵ�����ʹ�õİ뾶
    public float checkFallRadius = 0.1f;//�����棬��ǰ�������䶯��
    public LayerMask[] groundLayer;//����㼶

    #endregion ���������

    #region ǽ���

    public float checkWalloffset = 0.39f;//�������ĵ㵽�����Ե��ƫ��
    public Transform headCheck;//ͷ�����˵�λ��

    #endregion ǽ���

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