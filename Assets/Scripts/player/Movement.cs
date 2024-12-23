using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField, Header("行走速度")] public float walkSpeed;
    [SerializeField, Header("奔跑速度")] public float runSpeed;

    [SerializeField, Header("跳跃时施加的力")] public float jumpForce;
    [SerializeField, Header("墙跳时施加的力")] public float wallJumpForce;

    [SerializeField, Header("攻击所需能量")] private float attackEnergy;
    [SerializeField, Header("跳跃所需能量")] private float JumpEnergy;
    [SerializeField, Header("滑翔所需能量")] private float flyEnergy;
    [SerializeField, Header("爬墙所需能量")] private float grabWallEnergy;
    [SerializeField, Header("每秒回复能量")] private float replyEnergy;

    [Header("滑翔时受到的力")] public float forceWhenFly;
    [Header("在黑暗区域中，每过几秒才掉血")] public float lossHPTime;

    public GameObject WindBullet;
    public GameObject AttackSource;

    public bool isJump;//是否跳跃
    public bool isFall;//是否坠落
    public bool isDoubleJump;//是否二段跳
    public bool canAttack = true;//能够攻击
    public bool isInBrightScene;

    public int jumpCounter;

    public int magicalSamllTool = 0;
    public float flyTimeCounter;
    public float climbTimeCounter;
    public float replyEnergyTimeCounter;
    public float replyEnergyCD;
    public float lossHPInDarkTimeCounter;
    public float LangJumpTime;
    public float LangJumpCounter;

    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D haveFriction;
    public SpriteRenderer sr;

    public float fallMultiplier;//下落时受到的阻力
    public float lowJumpMultiplier;//小跳时受到的阻力

    public Vector2 dir;
    public Vector2 dirRaw;
    public float reverseGravityInt;

    private void Start()
    {
        Initial();
    }

    private void Update()
    {
        //判断该区域是否是不受光源影响区域
        isInBrightScene = false;
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5)
        {
            isInBrightScene = true;
        }

        //角色是否是死亡，未复活状态
        if (playerControlSystem.Instance.isDead)
        {
            playerControlSystem.Instance.rb.velocity = Vector2.zero;

            return;
        }
        if (playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.ATTACK)
        {
            playerControlSystem.Instance.rb.velocity = Vector2.zero;

            playerControlSystem.Instance.rb.gravityScale = 0;
            return;
        }
        else
        {
            playerControlSystem.Instance.rb.gravityScale = playerControlSystem.Instance.Gravity;
        }
        if (!playerControlSystem.Instance.canControl)
        {
            return;
        }

        if (playerControlSystem.Instance.isInteract) return;
        if (playerControlSystem.Instance.isRebirthing) return;

        if (playerControlSystem.Instance.isWebing)
        {
            playerControlSystem.Instance.rb.velocity = Vector2.zero;
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.WEB;
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        dir = new Vector2(x, y);
        dirRaw = new Vector2(xRaw, yRaw);

        //角色的转向
        if (playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.WallGrab &&
            !playerControlSystem.Instance.onWall)
        {
            if (x > 0)
            {
                if (playerControlSystem.Instance.isReverseControl)
                {
                    Flip(-1);
                }
                else
                {
                    Flip(1);
                }
            }
            if (x < 0)
            {
                if (playerControlSystem.Instance.isReverseControl)
                {
                    Flip(1);
                }
                else
                {
                    Flip(-1);
                }
            }
        }

        //在地面重置状态
        if (playerControlSystem.Instance.onGround && (jumpCounter > 0 || playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.none))
        {
            LangJumpCounter = 0;
            jumpCounter = 0;
            playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.none;
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.IDLE;
            isDoubleJump = false;
            isFall = false;
        }
        else if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab &&
            !playerControlSystem.Instance.onWall)
        {
            playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.WallJump;
        }

        //行走前，重置水平方向的速度
        //行走
        if (playerControlSystem.Instance.canMove && playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.WallGrab)
        {
            if (Input.GetKey(KeyCode.LeftShift)
              && playerControlSystem.Instance.onGround && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                Run();
            }
            else
            {
                Walk();
            }
        }

        //从地面上进行跳跃时，保证运行跳跃动画
        if (!playerControlSystem.Instance.onGround && playerControlSystem.Instance.rb.velocity.y > 0)
        {
            if (isDoubleJump)
            {
                playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.DOUBLEJUMP;
            }
            else if (playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.WALK
                || playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.RUN
                || playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.IDLE)
            {
                playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.JUMP;
            }
        }

        //判断是否下落
        if (!isDoubleJump && playerControlSystem.Instance.rb.velocity.y < 0 && !playerControlSystem.Instance.onGround &&
            (playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.JUMP ||
            playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.DOUBLEJUMP))
        {
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.FALL;
        }

        //土狼跳
        LangJumpCounter += Time.deltaTime;

        //跳跃
        if (!isJump && playerControlSystem.Instance.canJump && Input.GetKeyDown(KeyCode.Space))
        {
            if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab)
            {
                isJump = true;
                WallJump();
            }
            else if (playerControlSystem.Instance.onGround || LangJumpCounter < LangJumpTime
                //&& !playerControlSystem.Instance.onWall
                //&& playerControlSystem.Instance.energyPoint >= JumpEnergy
                )
            {
                LangJumpCounter = LangJumpTime + 1;
                isJump = true;
                Jump();
            }
            else if (LangJumpCounter > LangJumpTime && !isDoubleJump && !playerControlSystem.Instance.onGround && !playerControlSystem.Instance.onWall
                && playerControlSystem.Instance.energyPoint >= JumpEnergy
                //角色在明亮区域才能二段跳
                && (playerControlSystem.Instance.lightState == playerControlSystem.LightState.BRIGHT || isInBrightScene)
                )
            {
                isJump = true;
                DoubleJump();
            }
        }
        if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallJump)
        {
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.JUMP;
        }

        //下落
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            FallDown();
        }
        //角色在墙上，且灯光不为明亮，下落
        if (!isInBrightScene && playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab
            && playerControlSystem.Instance.lightState != playerControlSystem.LightState.BRIGHT)
        {
            FallDown();
        }

        //爬墙
        if (!isFall)
        {
            if (!playerControlSystem.Instance.onGround && playerControlSystem.Instance.onWall &&
            playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.WallGrab)
            {
                if ((playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.none ||
                    playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallJump ||
                    playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.Fly)
                    && playerControlSystem.Instance.energyPoint >= grabWallEnergy
                    && playerControlSystem.Instance.rb.velocity.y * reverseGravityInt < 0
                    //角色在明亮区域才能爬墙
                    && (playerControlSystem.Instance.lightState == playerControlSystem.LightState.BRIGHT || isInBrightScene)
                    )
                {
                    playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.WallGrab;
                }
                if (playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.WallJump)
                {
                    if (magicalSamllTool % 2 == 0)
                    {
                        playerControlSystem.Instance.rb.velocity = new Vector2(playerControlSystem.Instance.rb.velocity.x, 0);
                    }

                    magicalSamllTool++;
                }
            }
        }
        if (playerControlSystem.Instance.onGround || (playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.WallJump &&
                playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.WallGrab))
        {
            magicalSamllTool = 0;
        }
        if ((playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab &&
            playerControlSystem.Instance.onWall) && !playerControlSystem.Instance.onGround)
        {
            Climb();
        }
        else
        {
            climbTimeCounter = 0;
        }

        //滑翔
        if (!playerControlSystem.Instance.onGround)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (playerControlSystem.Instance.playerWallState != playerControlSystem.WallState.Fly)
                {
                    if (playerControlSystem.Instance.energyPoint >= flyEnergy)
                    {
                        playerControlSystem.Instance.rb.velocity = Vector2.zero;
                        playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.Fly;
                    }
                }
                else if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.Fly)
                {
                    playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.none;

                    playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.FALL;
                }
            }
        }
        else if (playerControlSystem.Instance.onGround || playerControlSystem.Instance.rb.velocity.y * reverseGravityInt > 0)
        {
            playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.none;
        }
        if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.Fly)
        {
            Fly();
        }

        //计算重力
        Gravity();

        //不在地面时，将人物的材质改为无摩擦力的材质
        if (playerControlSystem.Instance.onGround)
        {
            playerControlSystem.Instance.rb.sharedMaterial = haveFriction;
        }
        else
        {
            playerControlSystem.Instance.rb.sharedMaterial = haveFriction;
        }

        //在黑暗区域，持续扣血
        if (playerControlSystem.Instance.lightState == playerControlSystem.LightState.DARK && !isInBrightScene)
        {
            lossHPInDarkTimeCounter += Time.deltaTime;

            if (lossHPInDarkTimeCounter > lossHPTime)
            {
                lossHPInDarkTimeCounter = 0;
                BeAttackedSystem.Instance.lossHpInDark();
            }
        }
        else
        {
            lossHPInDarkTimeCounter = 0;
        }

        //死亡
        if (playerControlSystem.Instance.healthPoint <= 0)
        {
            Dead();
        }

        //攻击
        if (Input.GetMouseButtonDown(0) && playerControlSystem.Instance.energyPoint >= attackEnergy &&
            playerControlSystem.Instance.animationState != playerControlSystem.AnimationState.ATTACK && canAttack
            && playerControlSystem.Instance.animationState != playerControlSystem.AnimationState.FLY
            && playerControlSystem.Instance.animationState != playerControlSystem.AnimationState.CLIMB
            && !UISystem.Instance.isInUICanva)
        {
            canAttack = false;
            StopCoroutine(CanAttackAfterWhileAction(0f));
            StartCoroutine(CanAttackAfterWhileAction(2f));

            playerControlSystem.Instance.energyPoint -= attackEnergy;
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.ATTACK;
            playerControlSystem.Instance.canControl = false;
        }

        ReplyEnergy();
    }

    private void Attack()
    {
        Instantiate(WindBullet, AttackSource.transform.position, transform.rotation);
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.IDLE;
        playerControlSystem.Instance.rb.gravityScale = playerControlSystem.Instance.Gravity;
        playerControlSystem.Instance.canControl = true;
    }

    private void Dead()
    {
        if (transform.parent != null)
        {
            transform.parent = null;
        }

        playerControlSystem.Instance.am.Play("dead");
        playerControlSystem.Instance.isDead = true;
        StopAllCoroutines();
    }

    private void AnimationDeadToDisPlayDeadPlane()
    {
        //死亡后，不显示对话文本
        DialogSystem.Instance.contentTextGUI.gameObject.SetActive(false);

        //死亡后不显示血条和能量条
        UISystem.Instance.HpAndEnergy.SetActive(false);

        //死亡后显示死亡界面
        UISystem.Instance.DeadCanva.SetActive(true);
    }

    private void Initial()
    {
        isJump = false;
        reverseGravityInt = 1f;
        flyTimeCounter = 0f;
        climbTimeCounter = 0f;
        isFall = false;
        isDoubleJump = false;
        replyEnergyTimeCounter = 0f;
        lossHPInDarkTimeCounter = 0f;
        canAttack = true;
        jumpCounter = 0;
        sr = GetComponent<SpriteRenderer>();
        Flip(1);
    }

    private void FallDown()
    {
        isFall = true;

        playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.none;
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.FALL;
    }

    private void Climb()
    {
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.CLIMB;

        climbTimeCounter += Time.deltaTime;
        if (climbTimeCounter > 1f)
        {
            climbTimeCounter = 0;
            playerControlSystem.Instance.energyPoint += grabWallEnergy;
            if (playerControlSystem.Instance.energyPoint > 10)
            {
                playerControlSystem.Instance.energyPoint = 10;
            }
        }
    }

    private void Gravity()
    {
        //反转重力
        if (playerControlSystem.Instance.isReverseGravity)
        {
            reverseGravityInt = -1f;
        }
        else
        {
            reverseGravityInt = 1f;
        }

        //爬墙或滑翔的时候改变重力
        if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab)
        {
            playerControlSystem.Instance.rb.gravityScale = 0f;

            playerControlSystem.Instance.rb.velocity = new Vector2(playerControlSystem.Instance.rb.velocity.x, 0);
        }
        else if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.Fly && playerControlSystem.Instance.rb.velocity.y * reverseGravityInt < 0)
        {
            playerControlSystem.Instance.rb.gravityScale = playerControlSystem.Instance.Gravity * reverseGravityInt;
        }
        else
        {
            playerControlSystem.Instance.rb.gravityScale = playerControlSystem.Instance.Gravity * reverseGravityInt;
        }

        if (reverseGravityInt == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);  // 正常状态
        }
        else
        {
            transform.localScale = new Vector3(1, -1, 1); // 上下翻转
        }
    }

    private void Fly()
    {
        if (playerControlSystem.Instance.rb.velocity.y * reverseGravityInt < 0)
        {
            flyTimeCounter += Time.deltaTime;
            if (flyTimeCounter > 1f)
            {
                flyTimeCounter = 0f;
                playerControlSystem.Instance.energyPoint -= flyEnergy;
                if (playerControlSystem.Instance.energyPoint <= 0)
                {
                    FallDown();
                    playerControlSystem.Instance.energyPoint = 0;
                }
            }
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.FLY;

            playerControlSystem.Instance.rb.velocity = new Vector2(0, playerControlSystem.Instance.rb.velocity.y);

            playerControlSystem.Instance.rb.velocity = new Vector2((playerControlSystem.Instance.isReverseControl ? -1f : 1f) * dir.x * walkSpeed, -forceWhenFly * reverseGravityInt);
        }
        else
        {
            flyTimeCounter = 0f;
        }
        isFall = false;
    }

    private void Walk()
    {
        if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallJump)
        {
            if (playerControlSystem.Instance.isReverseControl)
            {
                playerControlSystem.Instance.rb.velocity = Vector2.Lerp(playerControlSystem.Instance.rb.velocity, (new Vector2(-dir.x * walkSpeed, playerControlSystem.Instance.rb.velocity.y)), 5 * Time.deltaTime);
            }
            else
            {
                playerControlSystem.Instance.rb.velocity = Vector2.Lerp(playerControlSystem.Instance.rb.velocity, (new Vector2(dir.x * walkSpeed, playerControlSystem.Instance.rb.velocity.y)), 5 * Time.deltaTime);
            }
        }
        else
        {
            if (playerControlSystem.Instance.isReverseControl)
            {
                playerControlSystem.Instance.rb.velocity = new Vector2(-dir.x * walkSpeed, playerControlSystem.Instance.rb.velocity.y);
            }
            else
            {
                playerControlSystem.Instance.rb.velocity = new Vector2(dir.x * walkSpeed, playerControlSystem.Instance.rb.velocity.y);
            }
        }
        if (dir.x != 0 && playerControlSystem.Instance.onGround)
        {
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.WALK;
        }
    }

    private void Run()
    {
        if (playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallJump)
        {
            if (playerControlSystem.Instance.isReverseControl)
            {
                playerControlSystem.Instance.rb.velocity = Vector2.Lerp(playerControlSystem.Instance.rb.velocity, (new Vector2(-dir.x * runSpeed, playerControlSystem.Instance.rb.velocity.y)), 5 * Time.deltaTime);
            }
            else
            {
                playerControlSystem.Instance.rb.velocity = Vector2.Lerp(playerControlSystem.Instance.rb.velocity, (new Vector2(dir.x * runSpeed, playerControlSystem.Instance.rb.velocity.y)), 5 * Time.deltaTime);
            }
        }
        else
        {
            if (playerControlSystem.Instance.isReverseControl)
            {
                playerControlSystem.Instance.rb.velocity = new Vector2(-dir.x * runSpeed, playerControlSystem.Instance.rb.velocity.y);
            }
            else
            {
                playerControlSystem.Instance.rb.velocity = new Vector2(dir.x * runSpeed, playerControlSystem.Instance.rb.velocity.y);
            }
        }

        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.RUN;
    }

    private void WallJump()
    {
        //墙跳
        Vector2 wallDir = playerControlSystem.Instance.onRightWall ? Vector2.left : Vector2.right;

        playerControlSystem.Instance.rb.velocity = Vector2.zero;
        playerControlSystem.Instance.rb.AddForce(wallDir * wallJumpForce, ForceMode2D.Impulse);
        playerControlSystem.Instance.rb.AddForce(Vector2.up * jumpForce * reverseGravityInt, ForceMode2D.Impulse);

        playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.WallJump;
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.JUMP;

        //不能跳跃
        CanNotJumpForWhile(0.1f);

        //不能行动
        CanNotMoveForWhile(0.2f);
        isJump = false;
        isFall = false;
    }

    private void Jump()
    {
        //playerControlSystem.Instance.energyPoint -= JumpEnergy;
        playerControlSystem.Instance.rb.velocity = new Vector2(playerControlSystem.Instance.rb.velocity.x, 0);
        playerControlSystem.Instance.rb.velocity += Vector2.up * jumpForce * reverseGravityInt;
        //playerControlSystem.Instance.rb.AddForce(Vector2.up * jumpForce * reverseGravityInt, ForceMode2D.Impulse);

        playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.none;

        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.JUMP;

        CanNotJumpForWhile(0.2f);

        jumpCounter++;

        isJump = false;
        isFall = false;
    }

    private void DoubleJump()
    {
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.DOUBLEJUMP;
        isDoubleJump = true;
        playerControlSystem.Instance.energyPoint -= JumpEnergy;

        playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.none;
        playerControlSystem.Instance.rb.velocity = new Vector2(playerControlSystem.Instance.rb.velocity.x, 0);
        playerControlSystem.Instance.rb.AddForce(Vector2.up * jumpForce * reverseGravityInt, ForceMode2D.Impulse);

        isDoubleJump = false;

        CanNotJumpForWhile(0.1f);

        isJump = false;
        isFall = false;
    }

    private void ReplyEnergy()
    {
        if (playerControlSystem.Instance.onGround)
        {
            replyEnergyTimeCounter += Time.deltaTime;
            replyEnergyCD += Time.deltaTime;
            if (replyEnergyCD > 1)
            {
                if (replyEnergyTimeCounter > 1)
                {
                    replyEnergyTimeCounter = 0;

                    //角色只能在明亮区域回复体力
                    if (playerControlSystem.Instance.lightState == playerControlSystem.LightState.BRIGHT || isInBrightScene)
                    {
                        playerControlSystem.Instance.energyPoint += replyEnergy;
                        if (playerControlSystem.Instance.energyPoint >= 10)
                        {
                            playerControlSystem.Instance.energyPoint = 10;
                        }
                    }
                }
            }
        }
        else
        {
            replyEnergyTimeCounter = 0;
            replyEnergyCD = 0;
        }
    }

    public void Flip(int side)
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
        if (!playerControlSystem.Instance.isRebirthing)
        {
            sr.flipX = (side == 1) ? false : true;
        }
    }

    public void CanNotMoveForWhile(float time)
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(time));
    }

    public void CanNotJumpForWhile(float time)
    {
        StopCoroutine(canJumpAfterWhile(0));
        StartCoroutine(canJumpAfterWhile(time));
    }

    private IEnumerator DisableMovement(float time)
    {
        playerControlSystem.Instance.canMove = false;
        yield return new WaitForSeconds(time);
        playerControlSystem.Instance.canMove = true;
    }

    private IEnumerator canJumpAfterWhile(float time)
    {
        playerControlSystem.Instance.canJump = false;
        yield return new WaitForSeconds(time);
        playerControlSystem.Instance.canJump = true;
    }

    private IEnumerator CanAttackAfterWhileAction(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            Dead();
        }
    }
}