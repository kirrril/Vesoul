using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class EntrySceneManager : MonoBehaviour
{
    [SerializeField]
    private UIentryScene uiEntryScene;
    public StartStop startStop;

    public event Action signInEmailDoesntMatch;
    public event Action signUpEmailDoesMatch;
    public event Action readyToEnterSignInPassword;
    public event Action readyToEnterSignUpPassword;
    public event Action signInWrongPassword;
    public event Action signInPasswordValidated;

    void Awake()
    {
        startStop = new StartStop();
        uiEntryScene.stopButtonClick += StopGame;
        uiEntryScene.onSignInEmailValidation += HandleSignInEmailInput;
        uiEntryScene.onSignUpEmailValidation += HandleSignUpEmailInput;
        uiEntryScene.onSignInPasswordInput += HandleSignInPasswordInput;
        uiEntryScene.signIn += HandleSignIn;
        uiEntryScene.signUp += HandleSignUp;
    }

    private void HandleSignInEmailInput(string emailInput)
    {
        if (PlayerPrefs.GetString("playerEmail") == null || emailInput != PlayerPrefs.GetString("playerEmail"))
        {
            signInEmailDoesntMatch?.Invoke();
        }
        else
        {
            readyToEnterSignInPassword?.Invoke();
        }
    }

    private void HandleSignInPasswordInput(string passwordInput)
    {
        if (passwordInput != PlayerPrefs.GetString("playerPassword"))
        {
            signInWrongPassword?.Invoke();
        }
        else
        {
            signInPasswordValidated?.Invoke();
        }
    }

    private void HandleSignUpEmailInput(string emailInput)
    {
        if (emailInput != PlayerPrefs.GetString("playerEmail"))
        {
            readyToEnterSignUpPassword?.Invoke();
        }
        else
        {
            signUpEmailDoesMatch?.Invoke();
        }
    }

    private void HandleSignIn()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void HandleSignUp(string email, string password)
    {
        PlayerPrefs.SetString("playerEmail", email);
        PlayerPrefs.SetString("playerPassword", password);
        SceneManager.LoadScene("GameScene");
    }

    private void StopGame()
    {
        startStop.HandleStopClick();
    }
}
