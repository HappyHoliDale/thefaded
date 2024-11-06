using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Data;
using UnityEngine;
using UnityEditor.Build.Content;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private string fileName;
    public static FileDataHandler fileDataHandler;

    Database data = new(); // 또는 = new Database();
    public List<ISavable> savableObjects = new();

    void Start()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        savableObjects = FindAllSavealbleObject();

        LoadGame();
    }

    public List<ISavable> FindAllSavealbleObject()
    {
        IEnumerable<ISavable> savables = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();
        return new List<ISavable>(savables);
    }

    public void SaveGame(bool SaveData)
    {
        if (SaveData)
        {
            foreach (var savableObject in savableObjects)
            {
                savableObject.SaveData(ref data);
            }
        }
        else data.Init();

        fileDataHandler.Save(data);
    }

    public void LoadGame()
    {
        data = fileDataHandler.Load();

        if (data == null)
        {
            Debug.Log("No Data Initialized. Initializing the data");
            data = new Database();
        }

        foreach (ISavable savableObject in savableObjects)
        {
            savableObject.LoadData(data);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame(true);
    }
}

public class FileDataHandler
{
    private string dataDirectoryPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirectoryPath, string dataFileName)
    {
        this.dataDirectoryPath = dataDirectoryPath;
        Debug.Log(dataDirectoryPath + " [ > ] " + dataFileName);
        this.dataFileName = dataFileName;
    }

    public Database Load()
    {
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        Database loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<Database>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        return loadedData;
    }

    public void Save(Database data)
    {
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}