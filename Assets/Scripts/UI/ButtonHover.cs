using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;

    [Header("正常状态下的按钮")] public Sprite normalButton;
    [Header("悬挂状态下的按钮")] public Sprite hoverButton;

    private void Start()
    {
        image = GetComponent<Image>();
        image.sprite = normalButton;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hoverButton;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = normalButton;
    }
}