using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchStage : MonoBehaviour
{
    public Image backImage;
    public Animator am;

    private void Start()
    {
        am = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            UISystem.Instance.isStarting = true;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (UISystem.Instance.isStarting)
            {
                UISystem.Instance.isStarting = false;
                BlackToWhite();
            }
        }
    }

    public void BlackToWhite()
    {
        DisPlayImg();
        Color color = backImage.color;
        color.a = 1f;
        backImage.color = color;
        am.Play("FadeOut");
    }

    public void WhiteToBlack()
    {
        DisPlayImg();
        Color color = backImage.color;
        color.a = 0f;
        backImage.color = color;
        am.Play("FadeIn");
    }

    public void DisPlayImg()
    {
        backImage.gameObject.SetActive(true);
    }

    public void HideImg()
    {
        backImage.gameObject.SetActive(false);
    }

    public void LoadScene()
    {
        //在移动平台上，转换场景时，不会报错
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (playerControlSystem.Instance.player.transform.parent != null)
            {
                playerControlSystem.Instance.player.transform.parent = null;
            }
        }

        //切换到下一个场景时，将这个场景的对话信息清除

        if(DialogSystem.Instance!=null)
            DialogSystem.Instance.completedDialogues.Clear();

        //获取当前场景信息，进入的场景为该场景的下一个
        int nextSceneNo = SceneManager.GetActiveScene().buildIndex + 1;

        //如果不在主菜单，则使用苏醒动画
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            UISystem.Instance.isStarting = true;
        }

        if (LoadController.Instance != null)
        {
            if (LoadController.Instance.isLoadGameFromMainMenu)
            {
                SceneManager.LoadScene(LoadController.Instance.savingData.SceneNo);
            }
            else
            {
                SceneManager.LoadScene(nextSceneNo);
            }
        }
        else
        {
            SceneManager.LoadScene(nextSceneNo);
        }
    }
}