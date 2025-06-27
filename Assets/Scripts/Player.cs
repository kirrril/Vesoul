using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;
    public static Player Instance;
    private PlayersRegistry playersRegistry;
    public PlayerProfile currentPlayer { get; private set; }
    public event Action<bool> emailDoesExist;
    public event Action<bool> authentificationSuccessful;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        entrySceneManager.checkSignIn += CheckSignIn;
        entrySceneManager.checkSignUpEmail += CheckSignUpEmail;
        entrySceneManager.savePlayerProfile += CreateNewPlayer;        
    }

    void Start()
    {
        CreatePlayersRegistry();
    }

    private void CreatePlayersRegistry()
    {
        string filePath = Application.persistentDataPath + $"/PlayersRegistry.json";
        if (!File.Exists(filePath))
        {
            playersRegistry = new PlayersRegistry();
            Debug.Log(filePath);
            string registryInit = JsonUtility.ToJson(playersRegistry);
            System.IO.File.WriteAllText(filePath, registryInit);
        }
        else
        {
            playersRegistry = JsonUtility.FromJson<PlayersRegistry>(System.IO.File.ReadAllText(filePath));

            foreach (string mail in playersRegistry.playersList) Debug.Log(mail);
        }
    }

    private void CheckSignIn(string email, string password)
    {
        if (playersRegistry.playersList.Contains(email.Replace("@", "_").Replace(".", "_")))
        {
            string filePath = Application.persistentDataPath + $"/{email.Replace("@", "_").Replace(".", "_")}.json";
            string playerProfile = System.IO.File.ReadAllText(filePath);
            currentPlayer = JsonUtility.FromJson<PlayerProfile>(playerProfile);

            if (currentPlayer.playerPassword == password)
            {
                authentificationSuccessful?.Invoke(true);
            }
            else
            {
                authentificationSuccessful?.Invoke(false);
            }
        }
        else
        {
            authentificationSuccessful?.Invoke(false);
        }
    }

    private void CheckSignUpEmail(string email)
    {
        string filePath = Application.persistentDataPath + $"/PlayersRegistry.json";

        foreach (string safeEmail in playersRegistry.playersList)
        {
            if (email.Replace("@", "_").Replace(".", "_") == safeEmail)
            {
                emailDoesExist?.Invoke(true);
            }
        }
    }

    private void CreateNewPlayer(string email, string password)
    {
        string safeEmail = email.Replace("@", "_").Replace(".", "_");

        SavePlayerProfile(safeEmail, password);
        AddPlayerToRegistry(safeEmail);
    }

    public void SavePlayerProfile(string safeEmail, string password)
    {
        currentPlayer = new PlayerProfile(safeEmail, password);
        string newPlayer = JsonUtility.ToJson(currentPlayer);
        string filePath = Application.persistentDataPath + $"/{safeEmail}.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, newPlayer);
    }

    public void UpdatePlayerProfile()
    {
        string profileUpdate = JsonUtility.ToJson(currentPlayer);
        string filePath = Application.persistentDataPath + $"/{currentPlayer.playerEmail.Replace("@", "_").Replace(".", "_")}.json";
        System.IO.File.WriteAllText(filePath, profileUpdate);
    }

    public void AddPlayerToRegistry(string safeEmail)
    {
        string filePath = Application.persistentDataPath + $"/PlayersRegistry.json";
        playersRegistry.playersList.Add(safeEmail);

        foreach (string mail in playersRegistry.playersList) Debug.Log(mail);

        string registryUpdate = JsonUtility.ToJson(playersRegistry);
        System.IO.File.WriteAllText(filePath, registryUpdate);
    }

    public void AddVisitedCity(string city)
    {
        currentPlayer.visitedCities.Add(city);
        currentPlayer.visitedCitiesCount++;
        currentPlayer.lastVisitedCity = city;
        currentPlayer.experience = IncreaseExperience();
        UpdatePlayerProfile();
    }

    private float IncreaseExperience()
    {
        return currentPlayer.visitedCities.Count / 10f;
    }

    [System.Serializable]
    public class PlayerProfile
    {
        public string playerEmail;
        public string playerPassword;
        public float experience;
        public List<string> visitedCities;
        public int visitedCitiesCount;
        public string lastVisitedCity;

        public PlayerProfile(string email, string password)
        {
            playerEmail = email;
            playerPassword = password;
            experience = 0f;
            visitedCities = new List<string>();
            visitedCitiesCount = 0;
            lastVisitedCity = string.Empty;
        }
    }

    [System.Serializable]
    public class PlayersRegistry
    {
        public List<string> playersList;

        public PlayersRegistry()
        {
            playersList = new List<string>();
        }
    }
}
