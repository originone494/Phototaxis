using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AwakeBlackCode : MonoBehaviour
{
    [Header("��ʧ��ʱ��")] public float hideTIme;
    public GameObject text;

    private Image img;
    private bool isHide = false;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHide = true;
            text.SetActive(false);
        }
        if (isHide)
        {
            float a = img.color.a;
            a -= Time.deltaTime / hideTIme;

            img.color = new Color(img.color.r, img.color.g, img.color.b, a);

            if (img.color.a <= 0)
            {
                isHide = false;

                //�ָ���ʾͼƬ
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);

                //�ָ���ʾ����
                text.SetActive(true);

                //�رո�����
                UISystem.Instance.AwakeBlackCodeopen.SetActive(false);
                LoadController.Instance.isStartRebirth = true;

                print("Awake is Start rebirth");
            }
        }
    }
}