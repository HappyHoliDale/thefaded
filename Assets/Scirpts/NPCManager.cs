using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData = new Dictionary<int, string[]>();

    void GenerateData()
    {
        //이현 NPC 대화
        talkData.Add(0, new string[] {
        "힘쎄고 강한 아침!",
        "내 이름을 물어본다면?",
        "내 이름은 이현."
    });
    }

    public string GetTalk(int id, int talkIndex)
    {
        return talkData[id][talkIndex];
    }
}
