using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance;

    [Header("死亡界面")] public GameObject DeadCanva;
    [Header("血条和能量条")] public GameObject HpAndEnergy;
    [Header("电影模式的黑块")] public GameObject DialogModeCanva;
    [Header("显示结局界面")] public GameObject EndCanva;
    [Header("苏醒界面黑块")] public GameObject AwakeBlackCodeopen;
    [Header("文本部分")] public GameObject DialogText;

    private Animator DialogAnimator;

    public bool isStarting;//用于决定是否使用渐变

    public bool isInUICanva;//是否处于UI界面

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