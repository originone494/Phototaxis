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
    public bool isDialogue;//�Ƿ����ڶԻ�
    public bool isStart;//�Ƿ�ʼ�Ի�
    public int index;//����
    public List<string> contentText;//�ı�
    public int contentLength = -1;//�ı�����

    public bool playerIsIntersect = false;//����Ƿ��ڽ���
    public bool isTeleport;//�Ƿ������һ������

    public GameObject upButton, downButton;//ѡ��ť
    public int isSelectUpButton;
    public GameObject inputButton;//�����
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

            //�����ı�
            contentText = eventBaseObject.contents;
            //��ȡ����
            contentLength = contentText.Count;
            //��������
            index = 0;

            //���ð�ť���ɼ�
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
                //1��ʾ����İ�ť
                if (isSelectUpButton == 1)
                {
                    eventBaseObject = eventBaseObject.branches[0];
                }

                //2��ʾ����İ�ť
                if (isSelectUpButton == 2)
                {
                    eventBaseObject = eventBaseObject.branches[1];
                }

                //�����ı�
                contentText = eventBaseObject.contents;
                //��ȡ����
                contentLength = contentText.Count;
                //��������
                index = 1;
                AnimationIndex = 0;

                //���ð�ť���ɼ�
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
                //�Ի������꣬�رնԻ�
                if (index == contentLength)
                {
                    Initial();//��ʼ��

                    if (playerControlSystem.Instance.isInteract)
                    {
                        //�����Ի�ģʽ
                        UISystem.Instance.EndDialog();

                        //��ɫ�˳�����ģʽ
                        playerControlSystem.Instance.isInteract = false;

                        //��ʾѪ����������
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
        //��ɫ�����ж�
        playerControlSystem.Instance.isInteract = true;
        //��ʾ�Ի�ģʽ�ڿ�
        UISystem.Instance.StartDialog();
        //��ʾ�ı�
        contentTextGUI.gameObject.SetActive(true);
        //�����ı���ɫ
        contentTextGUI.color = e.colors[0];
        //����Ѫ����������
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

    //���ִ�ӡ
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
            //�ݻ����еĵ���ģʽ
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