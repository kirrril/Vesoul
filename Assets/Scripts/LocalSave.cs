using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class LocalSave : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;
    string filePath;
    public event Action<List<string>> loadData;

    void Awake()
    {

    }

    void Start()
    {
        filePath = Application.persistentDataPath + "/PlayerProfile.json";
        if (!File.Exists(filePath)) UpdatePlayerProfile();
        LoadPlayerProfile();
        Player.Instance.saveData += UpdatePlayerProfile;
    }

    private void UpdatePlayerProfile()
    {
        filePath = Application.persistentDataPath + "/PlayerProfile.json";
        string profileUpdate = JsonUtility.ToJson(Player.Instance.currentPlayer);
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, profileUpdate);
    }

    private void LoadPlayerProfile()
    {
        filePath = Application.persistentDataPath + "/PlayerProfile.json";
        string playerProfile = System.IO.File.ReadAllText(filePath);
        var currentPlayer = JsonUtility.FromJson<Player.PlayerProfile>(playerProfile);

        loadData?.Invoke(currentPlayer.visitedCities);
    }
}
