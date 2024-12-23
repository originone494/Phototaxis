using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    public SwitchStage switchStage;
    public Button ContinueButton;

    private void Start()
    {
        if (File.Exists("data.a"))
        {
            ContinueButton.interactable = true;
            LoadController.Instance.LoadByDeserialization();
        }
        else
        {
            ContinueButton.interactable = false;
        }
    }

    public void PlayGame()
    {
        LoadController.Instance.isPlayGameFromMainMenu = true;

        switchStage.DisPlayImg();
        Color color = switchStage.backImage.color;
        color.a = 0f;
        switchStage.backImage.color = color;
        switchStage.am.Play("FadeIn");
    }

    public void ContinueGame()
    {
        LoadController.Instance.isLoadGameFromMainMenu = true;
        switchStage.DisPlayImg();
        Color color = switchStage.backImage.color;
        color.a = 0f;
        switchStage.backImage.color = color;
        switchStage.am.Play("FadeIn");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}