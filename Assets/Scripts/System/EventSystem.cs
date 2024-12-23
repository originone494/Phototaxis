using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance;

    public enum DialogEvent
    {
        NONE, DISPLAY_IMAGE, HIDE_IMAGE, BRANCH, LOAD_SCENE, ANIMATION, INPUT, SHAKE
    }

    public EventBaseObject eventBaseObject;
    public List<GameObject> images;
    public List<GameObject> playables;
    public int ImageIndex;
    public int AnimationIndex;

    public TextMeshProUGUI contentTextGUI;
    public float textSpeed;
    public float RealTextSpeed;
    public bool isDialogue;//是否正在对话
    public bool isStart;//是否开始对话
    public int index;//索引
    public List<string> contentText;//文本
    public int contentLength = -1;//文本长度

    public bool playerIsIntersect = false;//玩家是否在交流
    public bool isTeleport;//是否加载下一个场景

    public GameObject upButton, downButton;//选择按钮
    public int isSelectUpButton;
    public GameObject inputButton;//输入框
    public string inputContent;
    public bool isSelecting;
    public bool isAnimation;
    public bool isInputing;
    public bool isHaveInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject); return;
        }
        Instance = this;
    }

    private void Start()
    {
        contentTextGUI = UISystem.Instance.DialogText.GetComponent<TextMeshProUGUI>();
        Initial();
    }

    private void Update()
    {
        if (!isInputing && isHaveInput)
        {
            if (inputContent == "135")
            {
                eventBaseObject = eventBaseObject.branches[0];
            }
            else
            {
                eventBaseObject = eventBaseObject.branches[1];
            }

            //更新文本
            contentText = eventBaseObject.contents;
            //获取长度
            contentLength = contentText.Count;
            //更新索引
            index = 0;

            //设置按钮不可见
            inputButton.SetActive(false);

            inputContent = "";

            isHaveInput = false;
            isInputing = false;

            Next();
        }

        if (!isSelecting)
        {
            if (isSelectUpButton != -1)
            {
                //1表示上面的按钮
                if (isSelectUpButton == 1)
                {
                    eventBaseObject = eventBaseObject.branches[0];
                }

                //2表示下面的按钮
                if (isSelectUpButton == 2)
                {
                    eventBaseObject = eventBaseObject.branches[1];
                }

                //更新文本
                contentText = eventBaseObject.contents;
                //获取长度
                contentLength = contentText.Count;
                //更新索引
                index = 1;
                AnimationIndex = 0;

                //设置按钮不可见
                upButton.SetActive(false);
                downButton.SetActive(false);

                isSelectUpButton = -1;

                Next();
            }
        }

        if ((Input.GetKeyDown(KeyCode.E)) && isStart && !isSelecting && !isAnimation && !isInputing)
        {
            if (!isDialogue)
            {
                RealTextSpeed = textSpeed;
                //对话播放完，关闭对话
                if (index == contentLength)
                {
                    Initial();//初始化

                    if (playerControlSystem.Instance.isInteract)
                    {
                        //结束对话模式
                        UISystem.Instance.EndDialog();

                        //角色退出交流模式
                        playerControlSystem.Instance.isInteract = false;

                        //显示血条和体力条
                        UISystem.Instance.HpAndEnergy.SetActive(true);
                    }

                    playerIsIntersect = false;

                    contentTextGUI.gameObject.SetActive(false);
                }
                else
                {
                    if (eventBaseObject.whatEvents[index] == DialogEvent.DISPLAY_IMAGE)
                    {
                        UISystem.Instance.EndDialog();
                        images[ImageIndex++].SetActive(true);
                        Next();
                    }
                    else if (eventBaseObject.whatEvents[index] == DialogEvent.HIDE_IMAGE)
                    {
                        UISystem.Instance.StartDialog();
                        images[ImageIndex - 1].SetActive(false);
                        Next();
                    }
                    else if (eventBaseObject.whatEvents[index] == DialogEvent.LOAD_SCENE)
                    {
                        Next();
                        Initial();
                        playerControlSystem.Instance.isInteract = false;
                        UISystem.Instance.EndDialog();
                        UISystem.Instance.HpAndEnergy.SetActive(true);
                        contentTextGUI.gameObject.SetActive(false);
                        playerIsIntersect = false;
                        isTeleport = true;
                    }
                    else if (eventBaseObject.whatEvents[index] == DialogEvent.BRANCH)
                    {
                        TextMeshProUGUI upButtonText = upButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI downButtonText = downButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                        upButtonText.text = eventBaseObject.branches[0].contents[0];
                        downButtonText.text = eventBaseObject.branches[1].contents[0];

                        upButton.SetActive(true);
                        downButton.SetActive(true);

                        isSelecting = true;
                    }
                    else if (eventBaseObject.whatEvents[index] == DialogEvent.ANIMATION)
                    {
                        isAnimation = true;
                        UISystem.Instance.HpAndEnergy.SetActive(false);

                        UISystem.Instance.EndDialog();
                        PlayableDirector playable = playables[eventBaseObject.AnimationIndexes[AnimationIndex++]].GetComponent<PlayableDirector>();
                        playable.stopped += OnTimelineStopped;
                        Next();

                        playable.Play();
                    }
                    else if (eventBaseObject.whatEvents[index] == DialogEvent.INPUT)
                    {
                        isInputing = true;
                        inputButton.SetActive(true);
                    }
                    else if (eventBaseObject.whatEvents[index] == DialogEvent.NONE)
                    {
                        Next();
                    }
                }
            }
            else
            {
                RealTextSpeed = 0;
            }
        }
    }

    private void Initial()
    {
        isStart = false;
        isDialogue = false;
        index = 0;
        contentLength = -1;
        isSelectUpButton = -1;
        isSelecting = false;
        isAnimation = false;
        isInputing = false;
        isHaveInput = false;
        AnimationIndex = 0;
        ImageIndex = 0;

        eventBaseObject = null;
    }

    public void Play(bool isAuto, EventBaseObject e, List<GameObject> i, List<GameObject> p)
    {
        isStart = true;
        playerControlSystem.Instance.animationState = playerControlSystem.AnimationState.IDLE;
        //角色不能行动
        playerControlSystem.Instance.isInteract = true;
        //显示对话模式黑块
        UISystem.Instance.StartDialog();
        //显示文本
        contentTextGUI.gameObject.SetActive(true);
        //设置文本颜色
        contentTextGUI.color = e.colors[0];
        //隐藏血条和体力条
        UISystem.Instance.HpAndEnergy.SetActive(false);

        contentText = e.contents;
        contentLength = contentText.Count;

        eventBaseObject = e;
        images = i;
        playables = p;

        if (isAuto)
        {
            Next();
        }
    }

    private void Next()
    {
        contentTextGUI.color = eventBaseObject.colors[index];
        string node = contentText[index++];
        StopCoroutine(SetTextUI(node));
        StartCoroutine(SetTextUI(node));
    }

    //逐字打印
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

    public void ClickUp()
    {
        isSelecting = false;
        isSelectUpButton = 1;
    }

    public void ClickDown()
    {
        isSelecting = false;
        isSelectUpButton = 2;
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        director.stopped -= OnTimelineStopped;
        isAnimation = false;
        Debug.Log("timeline end");

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            //摧毁所有的单例模式
            playerControlSystem.Instance.Initial();
            Destroy(playerControlSystem.Instance.gameObject);
            playerControlSystem.Instance = null;
            Destroy(UISystem.Instance.gameObject);
            UISystem.Instance = null;
            Destroy(DialogSystem.Instance.gameObject);
            DialogSystem.Instance = null;
            SceneManager.LoadScene("MainMenu");
        }
    }
}