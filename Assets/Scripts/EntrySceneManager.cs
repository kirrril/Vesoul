using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class EntrySceneManager : MonoBehaviour
{
    [SerializeField]
    private UIentryScene uiEntryScene;

    [SerializeField]
    private CloudSave cloudSave;

    [SerializeField]
    private LocalSave localSave;

    public StartStop startStop;


    public event Action tryConnectCloud;
    public event Action cloudConnected;
    public event Action noCloudConnection;
    public event Action<string, string> trySignIn;
    public event Action<string, string> trySignUp;
    public event Action<string> displaySignInError;

    void Awake()
    {
        startStop = new StartStop();
        uiEntryScene.stopButtonClick += () => startStop.HandleStopClick();
        uiEntryScene.startButtonClick += () => SceneManager.LoadScene("GameScene");
        uiEntryScene.tryConnectCloud += () => tryConnectCloud?.Invoke();
        cloudSave.cloudConnected += () => cloudConnected?.Invoke();
        cloudSave.cloudConnectionFailed += () => noCloudConnection?.Invoke();
        uiEntryScene.signInButtonClick += HandleSignIn;
        uiEntryScene.signUpButtonClick += HandleSignUp;
        cloudSave.authentificationErrorWarning += HandleSignInError;
    }

    private void HandleSignIn(string email, string password)
    {
        trySignIn?.Invoke(email, password);
    }

    private void HandleSignUp(string email, string password)
    {
        trySignUp?.Invoke(email, password);
    }

    private void HandleSignInError(string errorMessage)
    {
        displaySignInError?.Invoke(errorMessage);
    }
}
