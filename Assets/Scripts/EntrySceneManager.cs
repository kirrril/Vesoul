using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class EntrySceneManager : MonoBehaviour
{
    [SerializeField]
    private UIentryScene uiEntryScene;

    public StartStop startStop;

    public event Action<string, string> trySignIn;
    public event Action<string, string> trySignUp;
    public event Action<string> displaySignInError;

    void Awake()
    {
        startStop = new StartStop();
        uiEntryScene.stopButtonClick += StopGame;

        uiEntryScene.signInButtonClick += HandleSignIn;
        uiEntryScene.signUpButtonClick += HandleSignUp;
    }

    void Start()
    {
        Player.Instance.errorWarning += HandleSignInError;
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

    private void StopGame()
    {
        startStop.HandleStopClick();
    }
}
