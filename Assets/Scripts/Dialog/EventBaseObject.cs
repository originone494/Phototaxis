using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[SerializeField, CreateAssetMenu(menuName = "创建对话", fileName = "对话")]
public class EventBaseObject : ScriptableObject
{
    [Header("对话内容")] public List<string> contents;
    [Header("分支内容")] public EventBaseObject[] branches;
    [Header("发生什么事件（和对话内容对应）")] public List<EventSystem.DialogEvent> whatEvents;
    [Header("文字颜色（和对话内容对应）")] public List<Color> colors;
    [Header("动画的编号")] public List<int> AnimationIndexes;
}