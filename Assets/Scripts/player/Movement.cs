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
    [SerializeField, Header("�����ٶ�")] public float walkSpeed;
    [SerializeField, Header("�����ٶ�")] public float runSpeed;

    [SerializeField, Header("��Ծʱʩ�ӵ���")] public float jumpForce;
    [SerializeField, Header("ǽ��ʱʩ�ӵ���")] public float wallJumpForce;

    [SerializeField, Header("������������")] private float attackEnergy;
    [SerializeField, Header("��Ծ��������")] private float JumpEnergy;
    [SerializeField, Header("������������")] private float flyEnergy;
    [SerializeField, Header("��ǽ��������")] private float grabWallEnergy;
    [SerializeField, Header("ÿ��ظ�����")] private float replyEnergy;

    [Header("����ʱ�ܵ�����")] public float forceWhenFly;
    [Header("�ںڰ������У�ÿ������ŵ�Ѫ")] public float lossHPTime;

    public GameObject WindBullet;
    public GameObject AttackSource;

    public bool isJump;//�Ƿ���Ծ
    public bool isFall;//�Ƿ�׹��
    public bool isDoubleJump;//�Ƿ������
    public bool canAttack = true;//�ܹ�����
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

    public float fallMultiplier;//����ʱ�ܵ�������
    public float lowJumpMultiplier;//С��ʱ�ܵ�������

    public Vector2 dir;
    public Vector2 dirRaw;
    public float reverseGravityInt;

    private void Start()
    {
        Initial();
    }

    private void Update()
    {
        //�жϸ������Ƿ��ǲ��ܹ�ԴӰ������
        isInBrightScene = false;
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5)
        {
            isInBrightScene = true;
        }

        //��ɫ�Ƿ���������δ����״̬
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

        //��ɫ��ת��
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

        //�ڵ�������״̬
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

        //����ǰ������ˮƽ������ٶ�
        //����
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

        //�ӵ����Ͻ�����Ծʱ����֤������Ծ����
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

        //�ж��Ƿ�����
        if (!isDoubleJump && playerControlSystem.Instance.rb.velocity.y < 0 && !playerControlSystem.Instance.onGround &&
            (playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.JUMP ||
            playerControlSystem.Instance.animationState == playerControlSystem.AnimationState.DOUBLEJUMP))
        {
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.FALL;
        }

        //������
        LangJumpCounter += Time.deltaTime;

        //��Ծ
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
                //��ɫ������������ܶ�����
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

        //����
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            FallDown();
        }
        //��ɫ��ǽ�ϣ��ҵƹⲻΪ����������
        if (!isInBrightScene && playerControlSystem.Instance.playerWallState == playerControlSystem.WallState.WallGrab
            && playerControlSystem.Instance.lightState != playerControlSystem.LightState.BRIGHT)
        {
            FallDown();
        }

        //��ǽ
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
                    //��ɫ���������������ǽ
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

        //����
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

        //��������
        Gravity();

        //���ڵ���ʱ��������Ĳ��ʸ�Ϊ��Ħ�����Ĳ���
        if (playerControlSystem.Instance.onGround)
        {
            playerControlSystem.Instance.rb.sharedMaterial = haveFriction;
        }
        else
        {
            playerControlSystem.Instance.rb.sharedMaterial = haveFriction;
        }

        //�ںڰ����򣬳�����Ѫ
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

        //����
        if (playerControlSystem.Instance.healthPoint <= 0)
        {
            Dead();
        }

        //����
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
        //�����󣬲���ʾ�Ի��ı�
        DialogSystem.Instance.contentTextGUI.gameObject.SetActive(false);

        //��������ʾѪ����������
        UISystem.Instance.HpAndEnergy.SetActive(false);

        //��������ʾ��������
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
        //��ת����
        if (playerControlSystem.Instance.isReverseGravity)
        {
            reverseGravityInt = -1f;
        }
        else
        {
            reverseGravityInt = 1f;
        }

        //��ǽ�����ʱ��ı�����
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
            transform.localScale = new Vector3(1, 1, 1);  // ����״̬
        }
        else
        {
            transform.localScale = new Vector3(1, -1, 1); // ���·�ת
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
        //ǽ��
        Vector2 wallDir = playerControlSystem.Instance.onRightWall ? Vector2.left : Vector2.right;

        playerControlSystem.Instance.rb.velocity = Vector2.zero;
        playerControlSystem.Instance.rb.AddForce(wallDir * wallJumpForce, ForceMode2D.Impulse);
        playerControlSystem.Instance.rb.AddForce(Vector2.up * jumpForce * reverseGravityInt, ForceMode2D.Impulse);

        playerControlSystem.Instance.playerWallState = playerControlSystem.WallState.WallJump;
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.JUMP;

        //������Ծ
        CanNotJumpForWhile(0.1f);

        //�����ж�
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

                    //��ɫֻ������������ظ�����
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