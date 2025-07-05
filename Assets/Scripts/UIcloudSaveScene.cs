using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class UIcloudSaveScene : MonoBehaviour
{
    [SerializeField]
    private EntrySceneManager entrySceneManager;

    private UIDocument entryUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button stopButton;
    private Button seConnecterButton;
    private VisualElement connectionErrorPanel;
    private Label connectionErrorWarning;
    private Button exitConnectionWarningButton;
    private Tab signInTab;
    private Tab signUpTab;
    private Label signInTabLabel;
    private Label signUpTabLabel;
    private TabView tabView;
    private VisualElement autorisationPanel;
    private Label signInUsernameWarning;
    private Label signUpUsernameWarning;
    private Label signInPasswordWarning;
    private Label signUpPasswordWarning;
    private TextField signInUsernameInput;
    private TextField signUpUsernameInput;
    private TextField signInPasswordInput;
    private TextField signUpPasswordInput;
    private TextField confirmPasswordInput;
    private Button signInButton;
    private Button signUpButton;
    private Button exitButton;
    public event Action startButtonClick;
    public event Action stopButtonClick;
    public event Action tryConnectCloud;
    public event Action<string, string> signInButtonClick;
    public event Action<string, string> signUpButtonClick;


    void Awake()
    {
        entryUIdocument = GetComponent<UIDocument>();
        root = entryUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        stopButton = root.Q<Button>("stopButton");
        seConnecterButton = root.Q<Button>("seConnecter");
        connectionErrorPanel = root.Q<VisualElement>("connectionErrorPanel");
        connectionErrorWarning = root.Q<Label>("connectionErrorWarning");
        exitConnectionWarningButton = root.Q<Button>("exitConnectionWarningButton");
        autorisationPanel = root.Q<VisualElement>("autorisationPanel");
        tabView = root.Q<TabView>("tabView");
        signInTab = root.Q<Tab>("seConnecterTab");
        signInTabLabel = signInTab.Q<Label>();
        signUpTab = root.Q<Tab>("creerCompteTab");
        signInUsernameWarning = root.Q<Label>("signInUsernameWarning");
        signUpUsernameWarning = root.Q<Label>("signUpUsernameWarning");
        signInPasswordWarning = root.Q<Label>("signInPasswordWarning");
        signUpPasswordWarning = root.Q<Label>("signUpPasswordWarning");
        signInUsernameInput = root.Q<TextField>("signInUsernameInput");
        signUpUsernameInput = root.Q<TextField>("signUpUsernameInput");
        signInPasswordInput = root.Q<TextField>("signInPasswordInput");
        signUpPasswordInput = root.Q<TextField>("signUpPasswordInput");
        confirmPasswordInput = root.Q<TextField>("confirmPasswordInput");
        signInButton = root.Q<Button>("signInButton");
        signUpButton = root.Q<Button>("signUpButton");
        exitButton = root.Q<Button>("exitButton");

        autorisationPanel.style.display = DisplayStyle.None;
        connectionErrorPanel.style.display = DisplayStyle.None;
        entrySceneManager.displaySignInError += DisplaySignInError;
        entrySceneManager.cloudConnected += DisplayAuthorisationPanel;
        entrySceneManager.noCloudConnection += DisplayConnectionErrorPanel;
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(evt => OnStartButtonClick());
        stopButton.RegisterCallback<ClickEvent>(evt => OnStopButtonClick());
        seConnecterButton.RegisterCallback<ClickEvent>(evt =>
        {
            tryConnectCloud?.Invoke();
            // autorisationPanel.style.display = DisplayStyle.Flex;
            // seConnecterButton.style.display = DisplayStyle.None;
        });
        exitConnectionWarningButton.RegisterCallback<ClickEvent>(evt => HideConnectionErrorPanel());
        exitButton.RegisterCallback<ClickEvent>(evt =>
        {
            autorisationPanel.style.display = DisplayStyle.None;
            seConnecterButton.style.display = DisplayStyle.Flex;
        });
        signInUsernameInput.RegisterValueChangedCallback(evt => OnSignInUsernameValueChange());
        signInPasswordInput.RegisterValueChangedCallback(evt => OnSignInPasswordValueChange());
        signInButton.RegisterCallback<ClickEvent>(evt => OnSignInButtonClick());
        signUpUsernameInput.RegisterValueChangedCallback(evt => OnSignUpUsernameValueChange());
        signUpPasswordInput.RegisterValueChangedCallback(evt => OnSignUpPasswordValueChange());
        confirmPasswordInput.RegisterValueChangedCallback(evt => OnConfirmPasswordValueChange());
        signUpButton.RegisterCallback<ClickEvent>(evt => OnSignUpButtonClick());
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(evt => OnStartButtonClick());
        stopButton.UnregisterCallback<ClickEvent>(evt => OnStopButtonClick());
        seConnecterButton.UnregisterCallback<ClickEvent>(evt =>
        {
            tryConnectCloud?.Invoke();
            // autorisationPanel.style.display = DisplayStyle.Flex;
            // seConnecterButton.style.display = DisplayStyle.None;
        });
        exitConnectionWarningButton.UnregisterCallback<ClickEvent>(evt => HideConnectionErrorPanel());
        exitButton.UnregisterCallback<ClickEvent>(evt =>
        {
            autorisationPanel.style.display = DisplayStyle.None;
            seConnecterButton.style.display = DisplayStyle.Flex;
        });
        signInUsernameInput.UnregisterValueChangedCallback(evt => OnSignInUsernameValueChange());
        signInPasswordInput.UnregisterValueChangedCallback(evt => OnSignInPasswordValueChange());
        signInButton.UnregisterCallback<ClickEvent>(evt => OnSignInButtonClick());
        signUpUsernameInput.UnregisterValueChangedCallback(evt => OnSignUpUsernameValueChange());
        signUpPasswordInput.UnregisterValueChangedCallback(evt => OnSignUpPasswordValueChange());
        confirmPasswordInput.UnregisterValueChangedCallback(evt => OnConfirmPasswordValueChange());
        signUpButton.UnregisterCallback<ClickEvent>(evt => OnSignUpButtonClick());
    }

    private void DisplayAuthorisationPanel()
    {
        autorisationPanel.style.display = DisplayStyle.Flex;
        signInUsernameWarning.text = "Le nom doit contenir de 3 à 20 lettres A-Z et a-z,\ndes chiffres et des symbols « . », « - », « @ » and « _ »";
        signInPasswordWarning.text = "Le mot de passe doit contenir entre 8 et 30 lettres, au moins une minuscule, une majuscule, un chiffre et un symbole";
        signInButton.SetEnabled(false);
        signUpUsernameWarning.text = "Le nom doit contenir de 3 à 20 lettres A-Z et a-z,\ndes chiffres et des symbols « . », « - », « @ » and « _ »";
        signUpPasswordWarning.text = "Le mot de passe doit contenir entre 8 et 30 lettres, au moins une minuscule, une majuscule, un chiffre et un symbole";
        signUpButton.SetEnabled(false);
    }

    private void OnSignInUsernameValueChange()
    {
        if (CheckUsername(signInUsernameInput.value))
        {
            signInUsernameWarning.text = "Le nom d'utilisateur conforme";
        }
        else
        {
            signInUsernameWarning.text = "Le nom doit contenir de 3 à 20 lettres A-Z et a-z,\ndes chiffres et des symbols « . », « - », « @ » and « _ »";
        }
    }

    private void OnSignInPasswordValueChange()
    {
        if (CheckPassword(signInPasswordInput.value))
        {
            signInPasswordWarning.text = "Le mot de passe conforme";
            signInButton.SetEnabled(true);
        }
        else
        {
            signInPasswordWarning.text = "Le mot de passe doit contenir entre 8 et 30 lettres, au moins une minuscule, une majuscule, un chiffre et un symbole";
            signInButton.SetEnabled(false);
        }
    }


    private void OnSignUpUsernameValueChange()
    {
        if (CheckUsername(signUpUsernameInput.value))
        {
            signUpUsernameWarning.text = "Le nom d'utilisateur conforme";
        }
        else
        {
            signUpUsernameWarning.text = "Le nom doit contenir de 3 à 20 lettres A-Z et a-z,\ndes chiffres et des symbols « . », « - », « @ » and « _ »";
        }
    }

    private void HandleSingUpUsernameMatching()
    {
        signUpUsernameWarning.text = "Un compte associé à cette adresse email existe déjà.\nConnectez-vous ou entrez une autre adresse email\npour créer un nouveau compte.";
    }

    private void OnSignUpPasswordValueChange()
    {
        if (CheckPassword(signUpPasswordInput.value))
        {
            signUpPasswordWarning.text = "Mot de passe conforme";
        }
        else
        {
            signUpPasswordWarning.text = "Le mot de passe doit contenir entre 8 et 30 lettres, au moins une minuscule, une majuscule, un chiffre et un symbole";
        }
        confirmPasswordInput.value = "";
    }

    private void OnConfirmPasswordValueChange()
    {
        if (confirmPasswordInput.value != "" && signUpPasswordInput.value != confirmPasswordInput.value)
        {
            confirmPasswordInput.style.color = new Color(1, 0, 0);
            signUpButton.SetEnabled(false);
        }
        else
        {
            confirmPasswordInput.style.color = new Color(0, 0, 0);
            signUpButton.SetEnabled(true);
        }
    }

    private bool CheckUsername(string usernameFieldValue)
    {
        string pattern = @"^[A-Za-z0-9._@-]{3,20}$";
        return Regex.IsMatch(usernameFieldValue, pattern);
    }

    private bool CheckPassword(string passwordFieldValue)
    {
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,30}$";
        return Regex.IsMatch(passwordFieldValue, pattern);
    }

    private void OnStartButtonClick()
    {
        startButtonClick?.Invoke();
    }

    private void OnStopButtonClick()
    {
        stopButtonClick?.Invoke();
    }

    private void DisplayConnectionErrorPanel()
    {
        seConnecterButton.style.display = DisplayStyle.None;
        autorisationPanel.style.display = DisplayStyle.None;
        connectionErrorPanel.style.display = DisplayStyle.Flex;
        connectionErrorWarning.text = "La connexion a échoué.\nVérifiez la connexion Internet et réessayez\nou jouez sans vous connecter.";
    }

    private void HideConnectionErrorPanel()
    {
        seConnecterButton.style.display = DisplayStyle.Flex;
        connectionErrorPanel.style.display = DisplayStyle.None;
    }

    private void OnSignInButtonClick()
    {
        signInButtonClick?.Invoke(signInUsernameInput.value, signInPasswordInput.value);
    }

    private void DisplaySignInError(string errorMessage)
    {
        signInButton.SetEnabled(false);
        signInUsernameWarning.text = errorMessage;
    }

    private void OnSignUpButtonClick()
    {
        signUpButtonClick?.Invoke(signUpUsernameInput.value, signUpPasswordInput.value);
    }
}