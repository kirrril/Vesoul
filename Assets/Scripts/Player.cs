using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;
    [SerializeField]
    private LocalSave localSave;
    [SerializeField]
    private CloudSave cloudSave;
    public static Player Instance;
    public PlayerProfile currentPlayer { get; private set; }

    public event Action saveData;


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

        currentPlayer = new PlayerProfile();

        localSave.loadData += LoadPlayerProfile;
    }

    private void LoadPlayerProfile(List<string> visitedCities)
    {
        currentPlayer.visitedCities = visitedCities;
    }

    public void AddVisitedCity(string city)
    {
        currentPlayer.visitedCities.Add(city);
        currentPlayer.lastVisitedCity = city;
        currentPlayer.experience = IncreaseExperience();

        saveData?.Invoke();
    }

    private float IncreaseExperience()
    {
        return currentPlayer.visitedCities.Count / 10f;
    }



    [System.Serializable]
    public class PlayerProfile
    {
        public List<string> visitedCities;
        public float experience;
        public string lastVisitedCity;

        public PlayerProfile()
        {
            visitedCities = new List<string>();
            experience = 0f;
            lastVisitedCity = string.Empty;
        }
    }
}
