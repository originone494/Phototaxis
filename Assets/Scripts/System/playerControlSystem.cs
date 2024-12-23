using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerControlSystem : MonoBehaviour
{
    public static playerControlSystem Instance;

    public GameObject player;

    /// <summary>
    /// �������
    /// </summary>
    public enum AnimationState
    {
        IDLE, WALK, RUN, JUMP, FALL, ATTACK, DEAD, DOUBLEJUMP, BEATTACK, CLIMB, FLY, WEB
    }

    public AnimationState animationState;

    /// <summary>
    /// ״̬���
    /// </summary>
    public enum WallState
    {
        Fly, WallGrab, WallJump, none
    }

    public WallState playerWallState;

    ///�ƹ�Ч��
    public enum LightState
    {
        DARK, DIM, BRIGHT
    }

    public LightState lightState;

    /// <summary>
    /// ������
    /// </summary>
    public bool onGround;//�Ƿ��ڵ���

    public bool onWall;//�Ƿ���ǽ��
    public bool onRightWall;//�Ƿ�����ǽ��
    public bool onLeftWall;//�Ƿ�����ǽ��
    public int wallSide;//ǽ�ĳ���

    public bool isTriggered = false;//��ɫ�Ƿ�����������

    /// <summary>
    /// bool���
    /// </summary>

    public bool canMove;//�Ƿ��ܹ��ƶ�
    public bool canJump;//�Ƿ��ܹ���Ծ
    public bool isReverseControl;//�Ƿ�ת����
    public bool isReverseGravity;//�Ƿ�ת����
    public bool isDead;//�Ƿ�����
    public bool isInteract;//�Ƿ����ڽ���
    public bool isWebing;
    public bool canControl;

    /// <summary>
    /// ������
    /// </summary>
    public Rigidbody2D rb;

    public Animator am;

    public SpriteRenderer sr;

    /// <summary>
    /// ��ֵ���
    /// </summary>
    public float healthPoint;

    public float energyPoint;

    [Header("��ʼ����ֵ")] public float originHealthPoint = 3f;

    [Header("��ʼ������")] public float originEnergyPoint = 10f;

    [Header("������С")] public float Gravity;

    /// <summary>
    /// �浵
    /// </summary>
    [Header("������")]
    public Vector2 rebirthPoint;

    public GameObject rebirthGameObject;

    public bool isRebirthing;

    /// <summary>
    /// ����
    /// </summary>
    public Color color;//��ɫԭ������ɫ

    public int StoneCount;//ʯ����

    private void Awake()
    {
        // ȷ�������ڳ������غ���Ȼ���ڼ���״̬
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

        //��ȡ��ʼ��ɫ
        color = sr.color;

        //���ý�ɫ͸��
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        canControl = false;

        //��ȡ��ʼ������λ��
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
                    //�ƶ����λ��
                    player.transform.position = new Vector2(save.savingPositionX, save.savingPositionY);
                    //��¼������
                    rebirthPoint = new Vector2(save.savingPositionX, save.savingPositionY);
                    //��ȡʯ������
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
                    //��¼������
                    rebirthPoint = new Vector2(save.savingPositionX, save.savingPositionY);
                    //��ȡʯ������
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

    //�����ó������ڳ������ҵ����
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
        //�����������������������
        UISystem.Instance.DeadCanva.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (player.name != "Player")
            {
                Debug.Log("��ҵ����ֲ���[Player]");
            }
            else
            {
                // ���»�ȡ���
                player.transform.position = rebirthPoint;
                rb = player.GetComponent<Rigidbody2D>();
                sr = player.GetComponent<SpriteRenderer>();
                am = player.GetComponent<Animator>();

                Debug.Log("�ɹ���ȡ���");
            }
        }
        else
        {
            Debug.Log("û���ҵ���Ҷ���");
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