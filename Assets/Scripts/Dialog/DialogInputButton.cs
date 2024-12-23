using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogInputButton : MonoBehaviour
{
    public TMP_InputField inputField;
    public int maxCharacterLimit = 3;

    private void Start()
    {
        inputField.characterLimit = maxCharacterLimit;
    }

    private void Update()
    {
        if (EventSystem.Instance.isInputing)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                EventSystem.Instance.inputContent = inputField.text;
                EventSystem.Instance.isInputing = false;
                EventSystem.Instance.isHaveInput = true;
            }
        }
    }
}