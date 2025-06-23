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
    public event Action<string> onSignInEmailValidation;
    public event Action<string> onSignUpEmailValidation;
    public event Action<string> onSignInPasswordInput;
    public event Action signIn;
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

        entrySceneManager.signInEmailDoesntMatch += HandleSingInEmailNotMatching;
        entrySceneManager.signUpEmailDoesMatch += HandleSingUpEmailMatching;
        entrySceneManager.readyToEnterSignInPassword += () =>
        {
            signInPasswordInput.SetEnabled(true);
            // signInPasswordInput.Focus();
            signInWarning.text = "";
        };
        entrySceneManager.signInWrongPassword += () =>
        {
            if (signInPasswordInput.value != string.Empty && signInPasswordInput.value != "Votre mot de passe ...")
            {
                signInWarning.text = "Mot de passe incorrecte !";
                signInButton.SetEnabled(false);
            }
            else
            {
                signInWarning.text = "";
            }
        };

        entrySceneManager.signInPasswordValidated += () =>
        {
            signInButton.SetEnabled(true);
            signInWarning.text = "";
        };
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
        if (signInWarning != null) signInWarning.text = "";
        // if (signInEmailInput != null) signInEmailInput.Focus();
        if (signInPasswordInput != null) signInPasswordInput.SetEnabled(false);
        if (signInButton != null) signInButton.SetEnabled(false);
        if (signUpWarning != null) signUpWarning.text = "";
        // if (signUpEmailInput != null) signUpEmailInput.Focus();
        if (signUpPasswordInput != null) signUpPasswordInput.SetEnabled(false);
        if (confirmPasswordInput != null) confirmPasswordInput.SetEnabled(false);
        if (signUpButton != null) signUpButton.SetEnabled(false);
    }

    private void OnSignInEmailValueChange()
    {
        signInPasswordInput.value = "Votre mot de passe ...";
        signInPasswordInput.SetEnabled(false);

        if (CheckEmail(signInEmailInput.value))
        {
            onSignInEmailValidation?.Invoke(signInEmailInput.value);
        }
        else if (!CheckEmail(signInEmailInput.value) && signInEmailInput.value != string.Empty && signInEmailInput.value != "Vorte adresse email ...")
        {
            signInWarning.text = "Une adresse email doit contenir @ et un point.";
        }
        else
        {
            signInWarning.text = "";
        }
    }

    private void HandleSingInEmailNotMatching()
    {
        signInPasswordInput.SetEnabled(false);
        signInWarning.text = "Adresse email inconnue. Corrigez-là ou créez un nouveau compte.";
    }

    private void OnSignInPasswordValueChange()
    {
        signUpPasswordInput.isPasswordField = true;
        onSignInPasswordInput?.Invoke(signInPasswordInput.value);
    }

    private void OnSignUpEmailValueChange()
    {
        signUpPasswordInput.value = "Votre mot de passe ...";
        signUpPasswordInput.SetEnabled(false);

        if (CheckEmail(signUpEmailInput.value))
        {
            onSignUpEmailValidation?.Invoke(signUpEmailInput.value);
            signUpPasswordInput.SetEnabled(true);
        }
        else
        {
            signUpWarning.text = "Une adresse email doit contenir @ et un point.";
            signUpPasswordInput.SetEnabled(false);
        }
    }

    private void HandleSingUpEmailMatching()
    {
        signUpWarning.text = "Un compte associé à cette adresse email existe déjà.";
    }

    private void OnSignUpPasswordValueChange()
    {
        if (signUpPasswordInput.value != string.Empty && signUpPasswordInput.value != "Votre mot de passe ...")
        {
            confirmPasswordInput.SetEnabled(true);
            confirmPasswordInput.value = "";
        }
        else
        {
            confirmPasswordInput.SetEnabled(false);
        }
    }

    private void OnConfirmPasswordValueChange()
    {
        if (signUpPasswordInput.value == confirmPasswordInput.value)
        {
            signUpButton.SetEnabled(true);
            signUpWarning.text = "Mot de passe confirmé";
            signUpWarning.style.color = new Color(0, 0, 0);
        }
        else
        {
            signUpWarning.text = "Confirmez le mot de passe.";
            signUpWarning.style.color = new Color(1, 0, 0);
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
        signIn?.Invoke();
    }

    private void OnSignUpButtonClick()
    {
        signUp?.Invoke(signUpEmailInput.value, signUpPasswordInput.value);
    }
}
