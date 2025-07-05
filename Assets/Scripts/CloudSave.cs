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

public class CloudSave : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;
    public event Action<string> authentificationErrorWarning;
    public event Action cloudConnectionFailed;
    public event Action cloudConnected;

    void Awake()
    {
        entrySceneManager.trySignIn += HandleSignIn;
        entrySceneManager.trySignUp += HandleSignUp;
        entrySceneManager.tryConnectCloud += CloudConnectionAttempt;
        // GameSceneManager.Instance.
    }

    async void CloudConnectionAttempt()
    {
        Debug.Log("CloudConnectionAttempt called");
        await InitializeCloudAsync();
    }

    async Task InitializeCloudAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await Awaitable.WaitForSecondsAsync(2000);
            cloudConnected?.Invoke();
            Debug.Log("Connexion établie");
        }
        catch (Exception e)
        {
            cloudConnectionFailed?.Invoke();
            Debug.LogException(e);
            Debug.Log("Echec connexion");
        }
    }

    private async void HandleSignIn(string username, string password)
    {
        Debug.Log("HandleSignIn called");
        AuthenticationService.Instance.SignOut();
        AuthenticationService.Instance.ClearSessionToken();
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

            authentificationErrorWarning?.Invoke("L'identifiant ou le mot de passe inconnu.\nCorrigez votre saisie ou créez un nouveau compte.");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }


    private async Task LoadPlayerStartGame(string username)
    {
        var key = new HashSet<string> { "visitedCities" };
        var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(key);

        cloudData.TryGetValue("visitedCities", out var visitedCitiesObj);
        Player.Instance.currentPlayer.visitedCities = visitedCitiesObj.Value.GetAs<List<string>>();

        SceneManager.LoadScene("GameScene");
    }


    private async void HandleSignUp(string username, string password)
    {
        AuthenticationService.Instance.SignOut();
        AuthenticationService.Instance.ClearSessionToken();
        await SignUpWithUsernamePasswordAsync(username, password);
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");

            await SaveData();
            SceneManager.LoadScene("GameScene");
        }
        catch (AuthenticationException ex)
        {
            authentificationErrorWarning?.Invoke("L'identifiant n'est pas disponible pour la création d'un nouveau compte");
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            authentificationErrorWarning?.Invoke("L'identifiant n'est pas disponible pour la création d'un nouveau compte");
            Debug.LogException(ex);
        }
    }




    public async Task SaveData()
    {
        var playerData = new Dictionary<string, object>
        {
            {"visitedCities", Player.Instance.currentPlayer.visitedCities},
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }

    private async Task OnApplicationQuit()
    {
        await SaveData();
    }

}
