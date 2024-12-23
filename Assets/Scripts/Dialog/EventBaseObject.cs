using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[SerializeField, CreateAssetMenu(menuName = "�����Ի�", fileName = "�Ի�")]
public class EventBaseObject : ScriptableObject
{
    [Header("�Ի�����")] public List<string> contents;
    [Header("��֧����")] public EventBaseObject[] branches;
    [Header("����ʲô�¼����ͶԻ����ݶ�Ӧ��")] public List<EventSystem.DialogEvent> whatEvents;
    [Header("������ɫ���ͶԻ����ݶ�Ӧ��")] public List<Color> colors;
    [Header("�����ı��")] public List<int> AnimationIndexes;
}