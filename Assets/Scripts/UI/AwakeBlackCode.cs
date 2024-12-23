using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AwakeBlackCode : MonoBehaviour
{
    [Header("消失的时间")] public float hideTIme;
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

                //恢复显示图片
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);

                //恢复显示字体
                text.SetActive(true);

                //关闭该物体
                UISystem.Instance.AwakeBlackCodeopen.SetActive(false);
                LoadController.Instance.isStartRebirth = true;

                print("Awake is Start rebirth");
            }
        }
    }
}