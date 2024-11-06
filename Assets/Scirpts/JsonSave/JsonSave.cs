using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]

public class Data
{
    public float time;
}

public class JsonSave : MonoBehaviour
{

    [SerializeField] public Data data = new();
    void Start()
    {

    }

    public void save()
    {
        // StreamWriter sw = new("a.txt");
        // sw.WriteLine(JsonUtility.ToJson(data));
        // sw.Close();

        using (StreamWriter sw = new("SaveInfo.txt"))
        {
            sw.WriteLine(JsonUtility.ToJson(data));
            sw.Close();
        }
    }

    public void Load()
    {
        // StreamReader sr = new("a.txt");
        // string str = sr.ReadToEnd();
        // data = JsonUtility.FromJson<Data>(str);
        using (StreamReader sr = new("SaveInfo.txt"))
        {
            string str = sr.ReadToEnd();
            data = JsonUtility.FromJson<Data>(str);
        }
    }

    void Update()
    {
        data.time += Time.deltaTime;
    }
}
