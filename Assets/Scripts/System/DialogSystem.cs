using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance;

    public TextMeshProUGUI contentTextGUI;
    public float textSpeed;
    public float nextTime;

    public float nextTimeCounter;
    public bool isAuto;

    public float RealTextSpeed;
    public bool isDialogue;//�Ƿ����ڶԻ�
    public bool isStart;//�Ƿ�ʼ�Ի�
    public string[] contentText;//�Ի�����
    public List<Color> textColors;//������ɫ

    private Coroutine curDialog;

    public HashSet<string> completedDialogues = new HashSet<string>();

    public bool HasDialogueOccurred(string dialogueID)
    {
        return completedDialogues.Contains(dialogueID);
    }

    public void StartDialogue(string dialogueID)
    {
        if (!completedDialogues.Contains(dialogueID))
        {
            completedDialogues.Add(dialogueID);
        }
    }

    private void Start()
    {
        Initial();
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

    private void OnEnable()
    {
        Initial();
    }

    public void Initial()
    {
        isStart = false;
        isDialogue = false;
        isAuto = false;
        nextTimeCounter = 0;
        contentTextGUI.text = "";
        contentText = new string[] { };
        textColors = new List<Color> { };
    }

    private void Update()
    {
        if (isStart)
        {
            if (isAuto)
            {
                nextTimeCounter += Time.deltaTime;
                if (nextTimeCounter > nextTime)
                {
                    nextTimeCounter = 0;
                    nextDialog();
                }
            }
            if ((Input.GetKeyDown(KeyCode.E)))
            {
                nextTimeCounter = 0;
                nextDialog();
            }
        }
    }

    private void nextDialog()
    {
        if (!isDialogue)
        {
            RealTextSpeed = textSpeed;
            if (contentText.Length == 0)
            {
                Initial();

                if (playerControlSystem.Instance.isInteract)
                {
                    //结束对话模式
                    UISystem.Instance.EndDialog();

                    //结束交流状态
                    playerControlSystem.Instance.isInteract = false;
                }

                UISystem.Instance.HpAndEnergy.SetActive(true);
                contentTextGUI.gameObject.SetActive(false);
            }
            else
            {
                next();
            }
        }
        else
        {
            RealTextSpeed = 0;
        }
    }

    public void play(string[] content, bool isCanMove, Color color, bool isAutoTrigger)
    {
        if (!isCanMove)
        {
            playerControlSystem.Instance.isInteract = true;

            UISystem.Instance.StartDialog();

            UISystem.Instance.HpAndEnergy.SetActive(false);
        }

        isStart = true;
        isAuto = isCanMove;

        if (contentText == null)
        {
            contentText = content;
            List<Color> colorList = Enumerable.Repeat(color, content.Length).ToList();
            textColors = colorList;
        }
        else
        {
            contentText = contentText.Concat(content).ToArray();
            List<Color> colorList = Enumerable.Repeat(color, content.Length).ToList();
            textColors.AddRange(colorList);
        }

        next();
        contentTextGUI.gameObject.SetActive(true);
    }

    private void next()
    {
        if (contentText != null)
        {
            print("dialog next <<<DialogSystem");
            string node = contentText[0];
            contentText = RemoveFirstElementFromArray(contentText);

            contentTextGUI.color = textColors[0];
            textColors.RemoveAt(0);

            curDialog = StartCoroutine(SetTextUI(node));
        }
        else
        {
            Debug.Log("contentText is null");
        }
    }

    private IEnumerator SetTextUI(string node)
    {
        isDialogue = true;
        contentTextGUI.text = "";
        for (int i = 0; i < node.Length; i++)
        {
            contentTextGUI.text += node[i];
            yield return new WaitForSeconds(RealTextSpeed);
        }
        isDialogue = false;
    }

    private string[] RemoveFirstElementFromArray(string[] array)
    {
        if (array == null || array.Length <= 1)
        {
            return new string[0];
        }

        string[] newArray = new string[array.Length - 1];

        for (int i = 1; i < array.Length; i++)
        {
            newArray[i - 1] = array[i];
        }

        return newArray;
    }
}