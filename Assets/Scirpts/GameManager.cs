using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text TalkText;
    public GameObject scanObject;
    public NPCManager NPCManager;
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        TalkText.text = "�̰�" + scanObject.name + "���� ����� ����";

    }
    void Talk()
    {
        //NPCManager.GetTalk(id, textIndex);
    }
}
