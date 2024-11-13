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
        TalkText.text = "이거" + scanObject.name + "같이 생겼다 히히";

    }
    void Talk()
    {
        //NPCManager.GetTalk(id, textIndex);
    }
}
