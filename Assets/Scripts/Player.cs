using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager gameSceneManager;
    public static Player Instance { get; private set; }

    public PlayerProfile playerProfile;

    private string playerEmail { get; set; }
    private string playerPassword { get; set; }
    public float experience { get; private set; }
    public List<string> visitedCities { get; private set; } = new List<string>();
    public int visitedCitiesCount { get; private set; }
    public string lastVisitedCity { get; private set; }


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
    }

    public void AddVisitedCity(string city)
    {
        visitedCities.Add(city);
        visitedCitiesCount++;
        lastVisitedCity = city;
        IncreaseExperience();
        SavePlayerData();
    }

    private void IncreaseExperience()
    {
        experience = visitedCities.Count / 10f;
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("visitedCitiesCount", visitedCitiesCount);

        for (int i = 0; i < visitedCitiesCount; i++)
        {
            PlayerPrefs.SetString($"visitedCity{i}", visitedCities[i]);
        }

        PlayerPrefs.SetFloat("playerExperience", experience);
        PlayerPrefs.SetString("lastVisitedCity", lastVisitedCity);

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        playerEmail = PlayerPrefs.GetString("playerEmail");
        experience = PlayerPrefs.GetFloat("playerExperience", 0f);
        visitedCitiesCount = PlayerPrefs.GetInt("visitedCitiesCount");

        if (visitedCitiesCount != 0)
        {
            for (int i = 0; i < PlayerPrefs.GetInt("visitedCitiesCount"); i++)
            {
                visitedCities.Add(PlayerPrefs.GetString($"visitedCity{i}"));
            }
        }
    }

    void OnApplicationQuit()
    {
        SavePlayerData();
    }

    [ContextMenu("ClearPlayerPrefs")]
    private void ClearPlayerPrefs()
    {
        for (int i = 0; i < visitedCitiesCount; i++)
        {
            PlayerPrefs.SetString($"visitedCity{i}", "");
        }
        PlayerPrefs.SetInt("visitedCitiesCount", 0);
        PlayerPrefs.SetFloat("playerExperience", 0f);
        PlayerPrefs.SetString("lastVisitedCity", string.Empty);
        PlayerPrefs.SetString("playerPassword", string.Empty);
        PlayerPrefs.SetString("playerEmail", string.Empty);
    }

    [ContextMenu("DebugPlayerPrefs")]
    private void DebugPlayerPrefs()
    {
        for (int i = 0; i < visitedCitiesCount; i++)
        {
            Debug.Log(PlayerPrefs.GetString($"visitedCity{i}"));
        }
        Debug.Log(PlayerPrefs.GetInt("visitedCitiesCount"));
        Debug.Log(PlayerPrefs.GetFloat("playerExperience"));
        Debug.Log(PlayerPrefs.GetString("lastVisitedCity"));
        Debug.Log(PlayerPrefs.GetString("playerPassword"));
        Debug.Log(PlayerPrefs.GetString("playerEmail"));
    }

    public void SavePlayerToJson()
    {
        string playerData = JsonUtility.ToJson(playerProfile);
        string filePath = Application.persistentDataPath + "/PlayerProfile.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, playerData);
    }
}

[System.Serializable]
public class PlayerProfile
{
    public string playerEmail;
    public string playerPassword;
    public float experience;
    public List<string> visitedCities = new List<string>();
    public int visitedCitiesCount;
    public string lastVisitedCity;
}

[System.Serializable]
public class RegisteredPlayers
{
    public string playerEmail;
}
