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
    public static Player Instance;
    public PlayerProfile currentPlayer { get; private set; }
    private bool cloudConnected;

    public event Action<string> errorWarning;

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
    }

    void Start()
    {
        CloudInitListenEvents();
    }

    async void CloudInitListenEvents()
    {
        await InitializeCloudAsync();

        if (cloudConnected)
        {
            entrySceneManager.trySignIn += HandleSignIn;
            entrySceneManager.trySignUp += HandleSignUp;
        }
    }

    async Task InitializeCloudAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            cloudConnected = true;
        }
        catch (Exception e)
        {
            cloudConnected = false;
            errorWarning?.Invoke("La connexion a échoué, vérifiez votre connextion Internet");
            Debug.LogException(e);
        }
    }

    private async void HandleSignIn(string username, string password)
    {
        await SignInWithUsernamePasswordAsync(username, password);
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");

            await LoadPlayerStartGame(username);
        }
        catch (AuthenticationException ex)
        {
            string fullMessage = ex.ToString();
            Debug.LogError(fullMessage);

            if (fullMessage.Contains("WRONG_USERNAME_PASSWORD") || fullMessage.Contains("Invalid username or password"))
            {
                errorWarning?.Invoke("L'identifiant ou le mot de passe est incorrect.");
            }
            else
            {
                errorWarning?.Invoke("Erreur d'authentification.");
            }
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            errorWarning?.Invoke("L'identifiant ou le mot de passe inconnu.\nCorrigez votre saisie ou créez un nouveau compte.");
        }
    }

    private async Task LoadPlayerStartGame(string username)
    {
        var keys = new HashSet<string> { "playerUsername", "experience", "visitedCities", "visitedCitiesCount", "lastVisitedCity" };
        var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        cloudData.TryGetValue("playerUsername", out var usernameObj);
        string loadedUsername = usernameObj.Value.GetAs<string>();

        if (username == loadedUsername)
        {
            currentPlayer = new PlayerProfile(loadedUsername);
            cloudData.TryGetValue("experience", out var experienceObj);
            currentPlayer.experience = experienceObj.Value.GetAs<float>();
            cloudData.TryGetValue("visitedCities", out var visitedCitiesObj);
            currentPlayer.visitedCities = visitedCitiesObj.Value.GetAs<List<string>>();
            cloudData.TryGetValue("visitedCitiesCount", out var visitedCitiesCountObj);
            currentPlayer.visitedCitiesCount = visitedCitiesCountObj.Value.GetAs<int>();
            cloudData.TryGetValue("lastVisitedCity", out var lastVisitedCityObj);
            currentPlayer.lastVisitedCity = lastVisitedCityObj.Value.GetAs<string>();

            SceneManager.LoadScene("GameScene");
        }
        else
        {
            errorWarning?.Invoke("L'identifiant ou le mot de passe incorrects");
        }
    }



    private async void HandleSignUp(string username, string password)
    {
        await SignUpWithUsernamePasswordAsync(username, password);
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");

            await CreatePlayerStartGame(username);
        }
        catch (AuthenticationException ex)
        {
            errorWarning?.Invoke("L'identifiant n'est pas disponible pour la création d'un nouveau compte");
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {

            errorWarning?.Invoke("L'identifiant n'est pas disponible pour la création d'un nouveau compte");
            Debug.LogException(ex);
        }
    }

    public async Task AddVisitedCity(string city)
    {
        currentPlayer.visitedCities.Add(city);
        currentPlayer.visitedCitiesCount++;
        currentPlayer.lastVisitedCity = city;
        currentPlayer.experience = IncreaseExperience();

        await SaveData();
    }

    private float IncreaseExperience()
    {
        return currentPlayer.visitedCities.Count / 10f;
    }

    private async Task CreatePlayerStartGame(string username)
    {
        currentPlayer = new PlayerProfile(username);

        await SaveData();

        SceneManager.LoadScene("GameScene");
    }

    public async Task SaveData()
    {
        var playerData = new Dictionary<string, object>
        {
            {"playerUsername", currentPlayer.playerUsername},
            {"experience", currentPlayer.experience},
            {"visitedCities", currentPlayer.visitedCities},
            {"visitedCitiesCount", currentPlayer.visitedCitiesCount},
            {"lastVisitedCity", currentPlayer.lastVisitedCity}
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }

    private async Task OnApplicationQuit()
    {
        await SaveData();
    }


    [System.Serializable]
    public class PlayerProfile
    {
        public string playerUsername;
        public float experience;
        public List<string> visitedCities;
        public int visitedCitiesCount;
        public string lastVisitedCity;

        public PlayerProfile(string username)
        {
            playerUsername = username;
            experience = 0f;
            visitedCities = new List<string>();
            visitedCitiesCount = 0;
            lastVisitedCity = string.Empty;
        }
    }
}
