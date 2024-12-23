using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerControlSystem : MonoBehaviour
{
    public static playerControlSystem Instance;

    public GameObject player;

    /// <summary>
    /// 动画相关
    /// </summary>
    public enum AnimationState
    {
        IDLE, WALK, RUN, JUMP, FALL, ATTACK, DEAD, DOUBLEJUMP, BEATTACK, CLIMB, FLY, WEB
    }

    public AnimationState animationState;

    /// <summary>
    /// 状态相关
    /// </summary>
    public enum WallState
    {
        Fly, WallGrab, WallJump, none
    }

    public WallState playerWallState;

    ///灯光效果
    public enum LightState
    {
        DARK, DIM, BRIGHT
    }

    public LightState lightState;

    /// <summary>
    /// 检测相关
    /// </summary>
    public bool onGround;//是否在地面

    public bool onWall;//是否在墙上
    public bool onRightWall;//是否在右墙上
    public bool onLeftWall;//是否在左墙上
    public int wallSide;//墙的朝向

    public bool isTriggered = false;//角色是否在重力光内

    /// <summary>
    /// bool相关
    /// </summary>

    public bool canMove;//是否能够移动
    public bool canJump;//是否能够跳跃
    public bool isReverseControl;//是否反转方向
    public bool isReverseGravity;//是否反转重力
    public bool isDead;//是否死亡
    public bool isInteract;//是否正在交流
    public bool isWebing;
    public bool canControl;

    /// <summary>
    /// 组件相关
    /// </summary>
    public Rigidbody2D rb;

    public Animator am;

    public SpriteRenderer sr;

    /// <summary>
    /// 数值相关
    /// </summary>
    public float healthPoint;

    public float energyPoint;

    [Header("初始生命值")] public float originHealthPoint = 3f;

    [Header("初始能量条")] public float originEnergyPoint = 10f;

    [Header("重力大小")] public float Gravity;

    /// <summary>
    /// 存档
    /// </summary>
    [Header("重生点")]
    public Vector2 rebirthPoint;

    public GameObject rebirthGameObject;

    public bool isRebirthing;

    /// <summary>
    /// 属性
    /// </summary>
    public Color color;//角色原来的颜色

    public int StoneCount;//石碑数

    private void Awake()
    {
        // 确保对象在场景加载后仍然处于激活状态
        gameObject.SetActive(true);
        print("playerControllerSystem enable");

        if (Instance != null && Instance != this)
        {
            print("repeat playerControllerSystem");

            Destroy(this.gameObject); return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        rb = player.GetComponent<Rigidbody2D>();

        sr = player.GetComponent<SpriteRenderer>();

        am = player.GetComponent<Animator>();

        //获取初始颜色
        color = sr.color;

        //设置角色透明
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        canControl = false;

        //获取初始重生点位置
        if (rebirthPoint == Vector2.zero)
        {
            rebirthPoint = player.transform.position;
        }

        Collider2D nearRebirth = Physics2D.OverlapCircle((Vector2)transform.position, 3f, LayerMask.GetMask("SpawnPoint"));
        if (nearRebirth != null)
        {
            rebirthGameObject = nearRebirth.gameObject;
        }
        else
        {
            print("nearRebirth is null");
        }

        StoneCount = 0;

        Initial();
        isRebirthing = false;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (LoadController.Instance != null)
            {
                if (LoadController.Instance.isLoadGameFromMainMenu)
                {
                    SaveGameObjects save = LoadController.Instance.savingData;
                    //移动玩家位置
                    player.transform.position = new Vector2(save.savingPositionX, save.savingPositionY);
                    //记录重生点
                    rebirthPoint = new Vector2(save.savingPositionX, save.savingPositionY);
                    //获取石碑数量
                    StoneCount = save.StoneCount;
                }
            }
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Player");
            if (rb == null) rb = player.GetComponent<Rigidbody2D>();
            if (sr == null) sr = player.GetComponent<SpriteRenderer>();
            if (am == null) am = player.GetComponent<Animator>();

            if (LoadController.Instance != null)
            {
                if (LoadController.Instance.isLoadGameFromMainMenu)
                {
                    canControl = false;
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                    SaveGameObjects save = LoadController.Instance.savingData;
                    player.transform.position = new Vector2(save.savingPositionX, save.savingPositionY);
                    //记录重生点
                    rebirthPoint = new Vector2(save.savingPositionX, save.savingPositionY);
                    //获取石碑数量
                    StoneCount = save.StoneCount;

                    UISystem.Instance.AwakeBlackCodeopen.SetActive(true);
                    LoadController.Instance.isLoadGameFromMainMenu = false;
                }
                else if (LoadController.Instance.isPlayGameFromMainMenu)
                {
                    UISystem.Instance.AwakeBlackCodeopen.SetActive(true);
                    LoadController.Instance.isPlayGameFromMainMenu = false;
                    canControl = false;
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
                }
                else if (LoadController.Instance.isStartRebirth)
                {
                    LoadController.Instance.isStartRebirth = false;
                    Collider2D nearRebirth = Physics2D.OverlapCircle((Vector2)player.transform.position, 2f, LayerMask.GetMask("SpawnPoint"));
                    if (nearRebirth != null)
                    {
                        rebirthGameObject = nearRebirth.gameObject;
                        Animator rebirthAm = rebirthGameObject.GetComponent<Animator>();

                        rebirthPoint = player.transform.position;

                        rebirthAm.Play("FirstAwake");
                    }
                    else
                    {
                        print("nearRebirth is null");
                    }
                }
            }
        }

        //if (SceneManager.GetActiveScene().buildIndex != 0)
        //{
        //    if (LoadController.Instance != null)
        //    {
        //        if (LoadController.Instance.isStartRebirth)
        //        {
        //            LoadController.Instance.isStartRebirth = false;

        //            Collider2D nearRebirth = Physics2D.OverlapCircle((Vector2)player.transform.position, 2f, LayerMask.GetMask("SpawnPoint"));
        //            if (nearRebirth != null)
        //            {
        //                rebirthGameObject = nearRebirth.gameObject;
        //                Animator rebirthAm = rebirthGameObject.GetComponent<Animator>();

        //                rebirthAm.Play("FirstAwake");
        //            }
        //            else
        //            {
        //                print("nearRebirth is null");
        //            }
        //        }
        //    }
        //}
    }

    public void Rebirth()
    {
        StartCoroutine(LoadSceneAndFindPlayer());
    }

    //在重置场景后，在场景中找到玩家
    private IEnumerator LoadSceneAndFindPlayer()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Initial();
        RebirthAction();
    }

    public void RebirthAction()
    {
        //场景加载完后，隐藏死亡界面
        UISystem.Instance.DeadCanva.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (player.name != "Player")
            {
                Debug.Log("玩家的名字不是[Player]");
            }
            else
            {
                // 重新获取组件
                player.transform.position = rebirthPoint;
                rb = player.GetComponent<Rigidbody2D>();
                sr = player.GetComponent<SpriteRenderer>();
                am = player.GetComponent<Animator>();

                Debug.Log("成功获取组件");
            }
        }
        else
        {
            Debug.Log("没有找到玩家对象");
        }

        isRebirthing = true;
        canControl = false;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);

        Collider2D nearRebirth = Physics2D.OverlapCircle((Vector2)player.transform.position, 2f, LayerMask.GetMask("SpawnPoint"));
        if (nearRebirth != null)
        {
            rebirthGameObject = nearRebirth.gameObject;
            Animator rebirthAm = rebirthGameObject.GetComponent<Animator>();
            rebirthAm.Play("CharAwake");
        }
        else
        {
            print("nearRebirth is null");
        }
    }

    public void Initial()
    {
        animationState = AnimationState.IDLE;
        playerWallState = WallState.none;
        canMove = true;
        canJump = true;
        isReverseControl = false;
        isReverseGravity = false;
        isDead = false;
        healthPoint = originHealthPoint;
        energyPoint = originEnergyPoint;
        rb.gravityScale = Gravity;
        isInteract = false;
        isWebing = false;
        canControl = true;
    }
}