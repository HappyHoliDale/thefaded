using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database
{
    public PlayerData playerData;
    public SkillTree st;
    public Database()
    {
        Init();
    }
    public void Init()
    {
        playerData = new PlayerData()
        { level = 1, coin = 0 };
    }
}
