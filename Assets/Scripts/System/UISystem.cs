using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance;

    [Header("��������")] public GameObject DeadCanva;
    [Header("Ѫ����������")] public GameObject HpAndEnergy;
    [Header("��Ӱģʽ�ĺڿ�")] public GameObject DialogModeCanva;
    [Header("��ʾ��ֽ���")] public GameObject EndCanva;
    [Header("���ѽ���ڿ�")] public GameObject AwakeBlackCodeopen;
    [Header("�ı�����")] public GameObject DialogText;

    private Animator DialogAnimator;

    public bool isStarting;//���ھ����Ƿ�ʹ�ý���

    public bool isInUICanva;//�Ƿ���UI����

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        DeadCanva.SetActive(false);
        HpAndEnergy.SetActive(false);

        DialogModeCanva.SetActive(false);
        DialogAnimator = DialogModeCanva.GetComponent<Animator>();
        DialogModeCanva.SetActive(false);

        EndCanva.SetActive(false);
    }

    public void StartDialog()
    {
        isInUICanva = true;

        DialogModeCanva.SetActive(true);
        DialogAnimator.Play("Display");
    }

    public void EndDialog()
    {
        isInUICanva = false;
        DialogAnimator.Play("Hide");
    }
}