using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuButton : MonoBehaviour
{
    private bool isPause;

    public GameObject pauseMenuButton;//暂停菜单

    public Button rebirthButton;//暂停菜单下的重生按钮

    private void Start()
    {
        isPause = false;
        pauseMenuButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPause)
            {
                UISystem.Instance.isInUICanva = true;

                isPause = true;

                Time.timeScale = 0;
                if (playerControlSystem.Instance != null)
                {
                    if (playerControlSystem.Instance.rebirthGameObject != null)
                    {
                        rebirthButton.interactable = true;
                    }
                    else
                    {
                        rebirthButton.interactable = false;
                    }
                }
                pauseMenuButton.SetActive(true);
            }
            else if (isPause)
            {
                UISystem.Instance.isInUICanva = false;

                isPause = false;
                Time.timeScale = 1;
                pauseMenuButton.SetActive(false);
            }
        }
    }

    public void BackGame()
    {
        UISystem.Instance.isInUICanva = false;

        Time.timeScale = 1;
        isPause = false;
        pauseMenuButton.SetActive(false);
    }

    public void BackMainMenu()
    {
        isPause = false;
        Time.timeScale = 1;
        pauseMenuButton.SetActive(false);
        UISystem.Instance.DialogText.SetActive(false);
        UISystem.Instance.DialogModeCanva.SetActive(false);
        UISystem.Instance.HpAndEnergy.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void Rebirth()
    {
        UISystem.Instance.isInUICanva = false;

        isPause = false;
        Time.timeScale = 1;

        UISystem.Instance.DialogText.SetActive(false);

        playerControlSystem.Instance.Rebirth();
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