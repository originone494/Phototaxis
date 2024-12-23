using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadCanva : MonoBehaviour
{
    public void RebirthButtonAction()
    {
        playerControlSystem.Instance.Rebirth();
    }

    public void BackMainMenuButton()
    {
        Destroy(playerControlSystem.Instance.gameObject);
        playerControlSystem.Instance = null;
        UISystem.Instance.DeadCanva.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void ExitGameButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}