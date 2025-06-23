using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIgameScene : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager gameSceneManager;
    private UIDocument gameUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button stopButton;
    private TextField playerInputField;
    private Button inputButton;
    private Label cityName;
    private Label alreadyPlayed;
    private Label comment;
    private ProgressBar remainingFuel;
    private StartStop startStop;
    public event Action<string> PlayerMadeInput;

    void Awake()
    {
        startStop = new StartStop();

        gameUIdocument = GetComponent<UIDocument>();
        root = gameUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        stopButton = root.Q<Button>("stopButton");
        playerInputField = root.Q<TextField>("playerInput");
        inputButton = root.Q<Button>("inputButton");
        cityName = root.Q<Label>("cityName");
        alreadyPlayed = root.Q<Label>("alreadyPlayedList");
        comment = root.Q<Label>("commentMessage");
        remainingFuel = root.Q<ProgressBar>("remainingFuel");
        startButton.SetEnabled(false);

        gameSceneManager.ShowCityToGuess += UpdateCity;
        gameSceneManager.AlreadyPlayedUpdate += UpdateAlreadyPlayed;
        gameSceneManager.FuelUpdate += UpdateFuelLevel;
        gameSceneManager.CommentUpdate += UpdateComment;

        playerInputField.style.opacity = 0;
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(evt => startStop.HandleStartClick());
        stopButton.RegisterCallback<ClickEvent>(evt => startStop.HandleStopClick());
        inputButton.RegisterCallback<ClickEvent>(evt => OnInputButtonClick());
        playerInputField.RegisterCallback<KeyUpEvent>(evt =>
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                OnPlayerInput();
                evt.StopPropagation();
                evt.StopImmediatePropagation();
            }
        });
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(evt => startStop.HandleStartClick());
        stopButton.UnregisterCallback<ClickEvent>(evt => startStop.HandleStopClick());
        inputButton.UnregisterCallback<ClickEvent>(evt => OnInputButtonClick());
        playerInputField.UnregisterCallback<KeyUpEvent>(evt => {});
    }

    private void OnInputButtonClick()
    {
        playerInputField.Focus();
        // TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    private void OnPlayerInput()
    {
        string playerInput = playerInputField.value;
        if (!string.IsNullOrEmpty(playerInput) && playerInput != " ")
        {
            PlayerMadeInput?.Invoke(playerInput);
            playerInputField.value = "";
            playerInputField.Blur();
        }
    }

    private void UpdateCity(string cityUpdated)
    {
        cityName.text = cityUpdated;
    }

    private void UpdateAlreadyPlayed(List<string> alreadyPlayedList)
    {
        string alreadyPlayedUpToDate = string.Empty;

        for (int i = 0; i < alreadyPlayedList.Count; i++)
        {
            if (alreadyPlayedUpToDate == "")
            {
                alreadyPlayedUpToDate += $"{alreadyPlayedList[i].ToUpper()}";
            }
            else
            {
                alreadyPlayedUpToDate += $", {alreadyPlayedList[i].ToUpper()}";
            }
        }

        if (alreadyPlayedUpToDate == "")
        {
            alreadyPlayed.text = "AUCUNE LETTRE N'A ETE JOUÉE POUR L'INSTANT";
        }
        else
        {
            alreadyPlayed.text = alreadyPlayedUpToDate;
        }
    }

    private void UpdateFuelLevel(int fuelLevel)
    {
        remainingFuel.value = fuelLevel;
    }

    private void UpdateComment(string commentUpdate)
    {
        comment.text = commentUpdate;
    }
}
