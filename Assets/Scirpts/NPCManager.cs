using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData = new Dictionary<int, string[]>();

    void GenerateData()
    {
        //���� NPC ��ȭ
        talkData.Add(0, new string[] {
        "����� ���� ��ħ!",
        "�� �̸��� ����ٸ�?",
        "�� �̸��� ����."
    });
    }

    public string GetTalk(int id, int talkIndex)
    {
        return talkData[id][talkIndex];
    }
}
