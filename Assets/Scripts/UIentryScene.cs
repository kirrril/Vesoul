using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;

public class UIentryScene : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;

    private UIDocument entryUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button stopButton;
    private Tab signInTab;
    private Tab signUpTab;
    private Label signInTabLabel;
    private Label signUpTabLabel;
    private TabView tabView;
    private VisualElement autorisationPanel;
    private Label signInWarning;
    private Label signUpWarning;
    private TextField signInEmailInput;
    private TextField signUpEmailInput;
    private TextField signInPasswordInput;
    private TextField signUpPasswordInput;
    private TextField confirmPasswordInput;
    private Button signInButton;
    private Button signUpButton;
    private Button exitButton;

    public event Action startButtonClick;
    public event Action stopButtonClick;
    public event Action<string, string> signIn;
    public event Action<string, string> signUp;


    void Awake()
    {
        entryUIdocument = GetComponent<UIDocument>();
        root = entryUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        stopButton = root.Q<Button>("stopButton");
        autorisationPanel = root.Q<VisualElement>("autorisationPanel");
        tabView = root.Q<TabView>("tabView");
        signInTab = root.Q<Tab>("seConnecterTab");
        signInTabLabel = signInTab.Q<Label>();
        signUpTab = root.Q<Tab>("creerCompteTab");
        signInWarning = root.Q<Label>("signInWarning");
        signUpWarning = root.Q<Label>("signUpWarning");
        signInEmailInput = root.Q<TextField>("signInEmailInput");
        signUpEmailInput = root.Q<TextField>("signUpEmailInput");
        signInPasswordInput = root.Q<TextField>("signInPasswordInput");
        signUpPasswordInput = root.Q<TextField>("signUpPasswordInput");
        confirmPasswordInput = root.Q<TextField>("confirmPasswordInput");
        signInButton = root.Q<Button>("signInButton");
        signUpButton = root.Q<Button>("signUpButton");
        exitButton = root.Q<Button>("exitButton");

        autorisationPanel.style.display = DisplayStyle.None;

        entrySceneManager.signUpEmailDoesMatch += HandleSingUpEmailMatching;
        entrySceneManager.authentificationFailed += OnAuthentificationFailed;
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(evt => OnStartButtonClick());
        stopButton.RegisterCallback<ClickEvent>(evt => OnStopButtonClick());
        exitButton.RegisterCallback<ClickEvent>(evt => OnExitButtonClick());
        signInEmailInput.RegisterValueChangedCallback(evt => OnSignInEmailValueChange());
        signInPasswordInput.RegisterValueChangedCallback(evt => OnSignInPasswordValueChange());
        signInButton.RegisterCallback<ClickEvent>(evt => OnSignInButtonClick());
        signUpEmailInput.RegisterValueChangedCallback(evt => OnSignUpEmailValueChange());
        signUpPasswordInput.RegisterValueChangedCallback(evt => OnSignUpPasswordValueChange());
        confirmPasswordInput.RegisterValueChangedCallback(evt => OnConfirmPasswordValueChange());
        signUpButton.RegisterCallback<ClickEvent>(evt => OnSignUpButtonClick());
        InitializeElements();
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(evt => OnStartButtonClick());
        stopButton.UnregisterCallback<ClickEvent>(evt => OnStopButtonClick());
        exitButton.UnregisterCallback<ClickEvent>(evt => OnExitButtonClick());
        signInEmailInput.UnregisterValueChangedCallback(evt => OnSignInEmailValueChange());
        signInPasswordInput.UnregisterValueChangedCallback(evt => OnSignInPasswordValueChange());
        signInButton.UnregisterCallback<ClickEvent>(evt => OnSignInButtonClick());
        signUpEmailInput.UnregisterValueChangedCallback(evt => OnSignUpEmailValueChange());
        signUpPasswordInput.UnregisterValueChangedCallback(evt => OnSignUpPasswordValueChange());
        confirmPasswordInput.UnregisterValueChangedCallback(evt => OnConfirmPasswordValueChange());
        signUpButton.UnregisterCallback<ClickEvent>(evt => OnSignUpButtonClick());
    }

    private void InitializeElements()
    {
        signInWarning.text = "";
        signInButton.SetEnabled(false);
        signUpWarning.text = "";
        signUpButton.SetEnabled(false);
        signInEmailInput.value = "Votre adresse email ...";
        signInPasswordInput.value = "Votre mot de passe ...";
        signUpEmailInput.value = "Votre adresse email ...";
        signUpPasswordInput.value = "Votre mot de passe ...";
        confirmPasswordInput.value = "Confirmez votre mot de passe ...";
    }

    private void OnSignInEmailValueChange()
    {
        if (CheckEmail(signInEmailInput.value))
        {
            signInWarning.text = "";
        }
        else
        {
            signInWarning.text = "Une adresse email doit contenir @ et un point.";
        }
    }

    private void OnSignInPasswordValueChange()
    {
        if (signInPasswordInput.value != "" && signInPasswordInput.value != "Votre mot de passe ...")
        {
            signInButton.SetEnabled(true);
        }
        else
        {
            signInButton.SetEnabled(false);
        }
    }


    private void OnSignUpEmailValueChange()
    {
        if (CheckEmail(signUpEmailInput.value))
        {
            signUpWarning.text = "";
        }
        else
        {
            signUpWarning.text = "Une adresse email doit contenir @ et un point.";
        }
    }

    private void HandleSingUpEmailMatching()
    {
        signUpWarning.text = "Un compte associé à cette adresse email existe déjà.\nConnectez-vous ou entrez une autre adresse email\npour créer un nouveau compte.";
    }

    private void OnSignUpPasswordValueChange()
    {
        confirmPasswordInput.value = "";
    }

    private void OnConfirmPasswordValueChange()
    {
        if (signUpPasswordInput.value == confirmPasswordInput.value)
        {
            confirmPasswordInput.style.color = new Color(0, 0, 0);
            signUpButton.SetEnabled(true);
        }
        else
        {
            confirmPasswordInput.style.color = new Color(1, 0, 0);
            signUpButton.SetEnabled(false);
        }
    }

    private bool CheckEmail(string emailFieldValue)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(emailFieldValue, pattern);
    }

    private void OnStartButtonClick()
    {
        startButton.style.display = DisplayStyle.None;
        stopButton.style.display = DisplayStyle.None;
        autorisationPanel.style.display = DisplayStyle.Flex;
    }

    private void OnStopButtonClick()
    {
        stopButtonClick?.Invoke();
    }

    private void OnExitButtonClick()
    {
        autorisationPanel.style.display = DisplayStyle.None;
        startButton.style.display = DisplayStyle.Flex;
        stopButton.style.display = DisplayStyle.Flex;
    }

    private void OnSignInButtonClick()
    {
        signIn?.Invoke(signInEmailInput.value, signInPasswordInput.value);
    }

    private void OnAuthentificationFailed()
    {
        signInButton.SetEnabled(false);
        signInWarning.text = "Adresse email ou mot de passe incorrectes.\nCorrigez votre saisie ou créez un nouveau compte.";
    }

    private void OnSignUpButtonClick()
    {
        signUp?.Invoke(signUpEmailInput.value, signUpPasswordInput.value);
    }
}