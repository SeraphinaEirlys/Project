using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

        ServiceLocator.Register(this);
    }

    private void Start()
    {
        StartCoroutine(LoadAfterInit());
    }

    private readonly List<IDataPersistence> persistenceObjects =
        new List<IDataPersistence>();

    public void SaveGame()
    {
        SaveData data = new SaveData();

        foreach (IDataPersistence persistenceObject in persistenceObjects)
        {
            persistenceObject.SaveData(data);
        }

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game saved to: " + savePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
            return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (IDataPersistence persistenceObject in persistenceObjects)
        {
            persistenceObject.LoadData(data);
        }
    }

    private IEnumerator LoadAfterInit()
    {
        yield return null;
        LoadGame();
    }

    public void Register(IDataPersistence persistenceObject)
    {
        if (!persistenceObjects.Contains(persistenceObject))
            persistenceObjects.Add(persistenceObject);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}