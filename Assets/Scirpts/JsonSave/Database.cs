using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database
{
    public PlayerData playerData;
    public Database()
    {
        Init();
    }
    public void Init()
    {
        playerData = new PlayerData()
        { savedTree = null, level = 1, coin = 0 };
    }
}
