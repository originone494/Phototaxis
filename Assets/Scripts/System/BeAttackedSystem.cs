using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeAttackedSystem : MonoBehaviour
{
    public static BeAttackedSystem Instance;

    [Header("����ʱ�䳤��")] public float knockBackTimeLength = 0.5f;
    [Header("��������")] public float knockBackForce = 3f;
    [Header("�ܻ��߶�����")] public float knockJumpForce = 12f;
    [Header("�޵�ʱ��")] public float CanNotBeAttackTime;
    [Header("�������к������ס��ʱ��")] public float WebingTime;
    [Header("�ںڰ������ܵ��˺���������ʱ��")] public float lossHpTimeInDark;
    [Header("�ںڰ������ܵ��˺���,��˸����ɫ")] public Color color;

    public ShakeCamera shakeCamera;

    private float knockBackCounter = 0;
    private bool fromAttackRecover;//�Ƿ�ӹ����лָ�����
    private bool CanNotBeAttack;//�Ƿ��ܹ����˺�
    private float CanNotBeAttackTimeCounter;
    private float lossHpTimeInDarkCounter;
    private bool lossHpInDary;

    private Coroutine Coroutine;

    private void Start()
    {
        fromAttackRecover = true;
        CanNotBeAttack = false;
        CanNotBeAttackTimeCounter = 0;
        lossHpTimeInDarkCounter = 0;
        lossHpInDary = false;
    }

    private void Update()
    {
        //��ȡ��������Ľű�
        shakeCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ShakeCamera>();

        if (!fromAttackRecover)
        {
            if (knockBackCounter > 0)
            {
                knockBackCounter -= Time.deltaTime;
            }
            else
            {
                if (!fromAttackRecover)
                {
                    playerControlSystem.Instance.canControl = true;
                    playerControlSystem.Instance.rb.velocity = Vector2.zero;
                    fromAttackRecover = true;
                    CanNotBeAttack = true;
                }
            }
        }

        if (CanNotBeAttack)
        {
            CanNotBeAttackTimeCounter += Time.deltaTime;

            float remainder = CanNotBeAttackTimeCounter % 0.3f;
            playerControlSystem.Instance.sr.enabled = remainder > 0.15f;
            if (CanNotBeAttackTimeCounter > CanNotBeAttackTime)
            {
                playerControlSystem.Instance.sr.enabled = true;

                CanNotBeAttack = false;
                CanNotBeAttackTimeCounter = 0;
            }

            if (playerControlSystem.Instance.isDead)
            {
                playerControlSystem.Instance.sr.enabled = true;
                CanNotBeAttack = false;

                CanNotBeAttackTimeCounter = 0;
            }
        }
        if (lossHpInDary)
        {
            lossHpTimeInDarkCounter += Time.deltaTime;

            if (lossHpTimeInDarkCounter > lossHpTimeInDark)
            {
                lossHpInDary = false;
                lossHpTimeInDarkCounter = 0;
                playerControlSystem.Instance.sr.color = playerControlSystem.Instance.color;
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void onAttacked(bool direction)
    {
        if (!CanNotBeAttack)
        {
            playerControlSystem.Instance.canControl = false;
            playerControlSystem.Instance.healthPoint -= 1;
            fromAttackRecover = false;
            knockBackCounter = knockBackTimeLength;
            playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.BEATTACK;
            if (direction)
            {
                playerControlSystem.Instance.rb.velocity = new Vector2(knockBackForce, knockJumpForce);
            }
            else
            {
                playerControlSystem.Instance.rb.velocity = new Vector2(-knockBackForce, knockJumpForce);
            }
            CanNotBeAttack = true;
        }
    }

    public void lossHpInDark()
    {
        if (playerControlSystem.Instance.isDead || playerControlSystem.Instance.isRebirthing)
        {
            return;
        }

        //����ʱ������Ļ
        shakeCamera.enabled = true;

        lossHpInDary = true;

        playerControlSystem.Instance.healthPoint--;
        playerControlSystem.Instance.sr.color = color;
    }

    public void PlayerBeAttackByWeb()
    {
        playerControlSystem.Instance.isWebing = true;
        playerControlSystem.Instance.healthPoint--;

        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
        }

        Coroutine = StartCoroutine(ExitPlayerWebingAction(WebingTime));
    }

    private IEnumerator ExitPlayerWebingAction(float time)
    {
        yield return new WaitForSeconds(time);
        playerControlSystem.Instance.isWebing = false;
    }
}