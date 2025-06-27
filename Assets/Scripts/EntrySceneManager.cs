using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class EntrySceneManager : MonoBehaviour
{
    [SerializeField]
    private UIentryScene uiEntryScene;
    [SerializeField]
    private Player player;
    public StartStop startStop;

    public event Action signUpEmailDoesMatch;
    public event Action signInWrong;
    public event Action<string, string> savePlayerProfile;
    public event Action<string, string> checkSignIn;
    public event Action<string> checkSignUpEmail;
    public event Action<string> signUpEmailExisting;
    public event Action authentificationFailed;

    void Awake()
    {
        startStop = new StartStop();
        uiEntryScene.stopButtonClick += StopGame;

        uiEntryScene.signIn += HandleSignIn;
        uiEntryScene.signUp += HandleSignUp;
        player.authentificationSuccessful += SignInAuthentification;
    }

    private void HandleSignIn(string email, string password)
    {
        checkSignIn?.Invoke(email, password);
    }

    private void SignInAuthentification(bool successful)
    {
        if (successful)
        {
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            authentificationFailed?.Invoke();
        }
    }

    private void HandleSignUp(string email, string password)
    {
        checkSignUpEmail?.Invoke(email);

        savePlayerProfile?.Invoke(email, password);

        SceneManager.LoadScene("GameScene");
    }

    private void StopGame()
    {
        startStop.HandleStopClick();
    }
}
