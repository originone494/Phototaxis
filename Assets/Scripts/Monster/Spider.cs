using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [Header("行走速度")] public float speed;
    [Header("追捕速度")] public float chaseSpeed;
    [Header("开始追逐玩家的最大距离")] public float detectDis;
    [Header("最大撞击玩家的判断距离")] public float bumpDis;
    [Header("冲撞攻击的cd")] public int bumpCD;
    [Header("远程攻击的cd")] public int shootCD;
    [Header("冲撞时的位移")] public float bumpTranslateDistance;
    [Header("眩晕时间")] public int dizzyTime;
    [Header("被击退时受到的力的大小")] public float backForce;
    [Header("到达地面的边缘后，停顿的时间")] public float turnAroundTime;
    [Header("处于下落状态多少秒死亡")] public float fallDieTime;

    [SerializeField] private bool canBump;
    [SerializeField] private bool canMove;//是否能够移动
    [SerializeField] private bool canShoot;
    [SerializeField] private bool isDizzy;//是否晕眩
    [SerializeField] private bool isFindPlayer;//是否找到玩家

    [SerializeField] private Collider2D player;

    [SerializeField] private float dir;
    [SerializeField] private float yDistance;//上下在这个范围内，被视为敌人

    private Animator am;
    private bool isUsingBump;
    private Collider2D coll;
    private float attackDir;
    public float dieTimeCounter;

    private Rigidbody2D rb;
    private SpriteRenderer sp;

    //地面检测相关
    public float checkGroundOffset;//为正

    public float checkGroundRayLength;//射线长度
    public float collisionRadius;

    //远程攻击相关
    public GameObject web;

    public Transform AttackSource;

    [SerializeField] private LayerMask layers;
    [SerializeField] private LayerMask monsterLayer;

    public Transform groundCheck;

    private enum MonsterState
    {
        IDLE, BUMPATTACK, WEBATTACK, DIZZY, WALK, CHASE
    }

    [SerializeField] private MonsterState state;

    private void Awake()
    {
        am = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        isFindPlayer = false;
        canMove = true;
        dir = -1;
        canBump = true;
        canShoot = true;
        isUsingBump = false;
        dieTimeCounter = 0;
    }

    private void Update()
    {
        print("bump:" + canBump);
        if (state != MonsterState.DIZZY)
        {
            state = MonsterState.IDLE;
        }
        else
        {
            SetAnimation();
            return;
        }

        DetectPlayer();
        Move();
        Attack();
        sp.flipX = dir > 0 ? true : false;

        SetAnimation();

        if (!Physics2D.OverlapCircle(groundCheck.position, collisionRadius, layers))
        {
            dieTimeCounter += Time.deltaTime;
            if (dieTimeCounter > fallDieTime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            dieTimeCounter = 0;
        }
    }

    private void DetectPlayer()
    {
        player = Physics2D.OverlapCircle((Vector2)transform.position, detectDis, LayerMask.GetMask("Player"));

        if (player != null)
        {
            Ray2D ray = new Ray2D(transform.position, player.transform.position - transform.position);

            RaycastHit2D hit = Physics2D.Linecast(ray.origin, ray.origin + ray.direction * detectDis, layers);

            Debug.DrawLine(ray.origin, ray.direction * detectDis, Color.red);

            if (hit.collider == null)
            {
                isFindPlayer = true;

                int direction;
                float distance = Mathf.Abs(transform.position.x - player.transform.position.x);

                if (distance > 1)
                {
                    if (player.transform.position.x < transform.position.x)
                    {
                        direction = -1;//玩家在敌人的左边
                    }
                    else
                    {
                        direction = 1;//玩家在敌人的右边
                    }
                    if (direction != dir)//表示方向不一致
                    {
                        dir = dir > 0 ? -1 : 1;
                    }
                }
            }
        }
        else
        {
            isFindPlayer = false;
        }
    }

    private void Move()
    {
        if (!canMove)
        {
            return;
        }

        if (!isFindPlayer)
        {
            PlatMove();
        }
        else
        {
            chasePlayer();
        }

        if (rb.velocity.x > 0.1f || rb.velocity.x < -0.1f)
        {
            state = isFindPlayer ? MonsterState.CHASE : MonsterState.WALK;
        }
    }

    private void chasePlayer()
    {
        float distance = Mathf.Abs(transform.position.x - player.transform.position.x);

        if (distance > 1f)
        {
            if (dir < 0)
            {
                if (Physics2D.Linecast(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x - checkGroundOffset, transform.position.y - checkGroundRayLength), LayerMask.GetMask("Ground")))//抵达边境
                {
                    rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
                }
            }
            else if (dir > 0)
            {
                if (Physics2D.Linecast(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
                new Vector2(transform.position.x + checkGroundOffset, transform.position.y - checkGroundRayLength), LayerMask.GetMask("Ground")))//抵达边境
                {
                    rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
                }
            }
        }
    }

    private void PlatMove()
    {
        Physics2D.queriesStartInColliders = true;

        if (dir < 0)
        {
            //如果下面没地面，说明到达边界
            if (!Physics2D.Linecast(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
        new Vector2(transform.position.x - checkGroundOffset, transform.position.y - checkGroundRayLength), LayerMask.GetMask("Ground")))//抵达边境
            {
                CallCanNotMoveForWhile(turnAroundTime);
                return;
            }
            //如果上面有地面，说明不能通过
            else if (Physics2D.Linecast(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
        new Vector2(transform.position.x - checkGroundOffset, transform.position.y + checkGroundRayLength), LayerMask.GetMask("Ground")))//抵达边境
            {
                CallCanNotMoveForWhile(turnAroundTime);
                return;
            }
            //检测蜘蛛
            else if (Physics2D.Linecast(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x - checkGroundOffset * 2, transform.position.y + checkGroundRayLength), monsterLayer))//抵达边境
            {
                dir = 1;
                return;
            }
        }
        else if (dir > 0)
        {
            if (!Physics2D.Linecast(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x + checkGroundOffset, transform.position.y - checkGroundRayLength), LayerMask.GetMask("Ground")))//抵达边境
            {
                CallCanNotMoveForWhile(turnAroundTime);
                return;
            }
            else if (Physics2D.Linecast(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x + checkGroundOffset, transform.position.y + checkGroundRayLength), LayerMask.GetMask("Ground")))//抵达边境
            {
                CallCanNotMoveForWhile(turnAroundTime);
                return;
            }
            //检测蜘蛛
            else if (Physics2D.Linecast(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x + checkGroundOffset * 2, transform.position.y + checkGroundRayLength), monsterLayer))//抵达边境
            {
                dir = -1;
                return;
            }
        }

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    private void Attack()
    {
        if (isFindPlayer)
        {
            if (!canShoot || !canBump)
            {
                state = MonsterState.CHASE;
            }

            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < bumpDis)
            {
                if (canBump)
                {
                    state = MonsterState.BUMPATTACK;
                    attackDir = dir;
                }
            }
            else if (distance > bumpDis && distance < detectDis)
            {
                if (canShoot)
                {
                    state = MonsterState.WEBATTACK;
                }
            }

            if (isUsingBump)
            {
                if (dir > 0)
                {
                    if (state != MonsterState.DIZZY)
                    {
                        transform.Translate(Vector3.right * bumpTranslateDistance * Time.deltaTime);
                    }
                }
                else if (dir < 0)
                {
                    if (state != MonsterState.DIZZY)
                    {
                        transform.Translate(-Vector3.right * bumpTranslateDistance * Time.deltaTime);
                    }
                }
            }
        }
    }

    private void BumpAttack()
    {
        StartCoroutine(BumpStartCD(bumpCD));

        isUsingBump = true;
        print("bump attack");
    }

    private void WebAttack()
    {
        StartCoroutine(ShootWebStartCD(shootCD));
        Instantiate(web, AttackSource.transform.position, transform.rotation);
        print("web attack");
    }

    private void SetAllStateFalse()
    {
        am.SetBool("walk", false);
        am.SetBool("far", false);
        am.SetBool("chase", false);
        am.SetBool("dizzy", false);
        am.SetBool("near", false);
    }

    private void SetAnimation()
    {
        switch (state)
        {
            case MonsterState.DIZZY:
                SetAllStateFalse();
                am.SetBool("dizzy", true);
                break;

            case MonsterState.WEBATTACK:
                SetAllStateFalse();
                am.SetBool("far", true);
                break;

            case MonsterState.BUMPATTACK:
                SetAllStateFalse();
                am.SetBool("near", true);
                break;

            case MonsterState.WALK:
                SetAllStateFalse();
                am.SetBool("walk", true);
                break;

            case MonsterState.CHASE:
                SetAllStateFalse();
                am.SetBool("chase", true);
                break;

            case MonsterState.IDLE:
                SetAllStateFalse();
                break;
        }
    }

    private void CallCanNotMoveForWhile(float time)
    {
        StopCoroutine(canNotMoveForWhile(0));
        StartCoroutine(canNotMoveForWhile(time));
    }

    private IEnumerator canNotMoveForWhile(float time)
    {
        canMove = false;
        state = MonsterState.IDLE;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(time);
        canMove = true;
        dir = dir > 0 ? -1 : 1;
    }

    private IEnumerator BumpStartCD(float time)
    {
        canBump = false;
        yield return new WaitForSeconds(time);
        canBump = true;
        print("bump cd over");
    }

    private IEnumerator ShootWebStartCD(float time)
    {
        canShoot = false;
        yield return new WaitForSeconds(time);
        canShoot = true;
    }

    private void OnDrawGizmos()//绘制辅助线
    {
        Gizmos.color = Color.red;//辅助线颜色
        //Gizmos.DrawWireSphere((Vector2)transform.position, detectDis);
        //Gizmos.DrawWireSphere((Vector2)transform.position, bumpDis);

        Gizmos.DrawLine(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x - checkGroundOffset, transform.position.y - checkGroundRayLength));

        Gizmos.DrawLine(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
        new Vector2(transform.position.x - checkGroundOffset, transform.position.y + checkGroundRayLength));

        Gizmos.DrawLine(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x + checkGroundOffset, transform.position.y - checkGroundRayLength));

        Gizmos.DrawLine(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x + checkGroundOffset, transform.position.y + checkGroundRayLength));

        Gizmos.DrawLine(new Vector2(transform.position.x + checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x + checkGroundOffset * 2, transform.position.y));
        Gizmos.DrawLine(new Vector2(transform.position.x - checkGroundOffset, transform.position.y),
            new Vector2(transform.position.x - checkGroundOffset * 2, transform.position.y));
    }

    //一个动作开始时，不能行动
    private void AnimationCanNotMoveEvent()
    {
        canMove = false;
    }

    //一个动作结束后，可以继续行动
    private void AnimationStartMoveEvent()
    {
        canMove = true;
        isUsingBump = false;
    }

    private IEnumerator DizzyCanNotMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        state = MonsterState.IDLE;
    }

    private void CallDizzyCanNotMoveAction(float time)
    {
        StopCoroutine(DizzyCanNotMove(0f));
        StartCoroutine(DizzyCanNotMove(time));
    }

    private void Dizzy()
    {
        state = MonsterState.DIZZY;
        CallDizzyCanNotMoveAction(dizzyTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isUsingBump && state != MonsterState.DIZZY)
        {
            if (collision.CompareTag("Player"))
            {
                BeAttackedSystem.Instance.onAttacked(playerControlSystem.Instance.sr.flipX);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack") && collision != coll)
        {
            coll = collision;
            float direction;
            if (playerControlSystem.Instance.player.transform.position.x < transform.position.x)
            {
                direction = 1;//玩家在敌人的左边
            }
            else
            {
                direction = -1;//玩家在敌人的右边
            }
            Dizzy();
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(direction * backForce, 0));
        }
    }
}