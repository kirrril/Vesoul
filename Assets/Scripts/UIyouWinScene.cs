using UnityEngine;
using UnityEngine.UIElements;

public class UIyouWinScene : MonoBehaviour
{
    [SerializeField]
    private YouWinSceneManager youWinSceneManager;
    private UIDocument youWinUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button stopButton;
    private VisualElement background;
    private Label comment;

    private StartStop startStop;

    void Awake()
    {
        startStop = new StartStop();

        youWinUIdocument = GetComponent<UIDocument>();
        root = youWinUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        stopButton = root.Q<Button>("stopButton");
        background = root.Q<VisualElement>("background");
        comment = root.Q<Label>("commentMessage");

        youWinSceneManager.getCityView += DisplayViewAndGreeting;
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(evt => startStop.HandleStartClick());
        stopButton.RegisterCallback<ClickEvent>(evt => startStop.HandleStopClick());
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(evt => startStop.HandleStartClick());
        stopButton.UnregisterCallback<ClickEvent>(evt => startStop.HandleStopClick());
    }

    private void DisplayViewAndGreeting(Texture2D cityView, string cityName)
    {
        Sprite sprite = Sprite.Create(cityView, new Rect(0, 0, cityView.width, cityView.height), Vector2.one * 0.5f);
        background.style.backgroundImage = new StyleBackground(sprite);
        comment.text = $"BIENVENU À {cityName} !\nAPPUYEZ SUR START GAME\nPOUR CONTINUER LE VOYAGE";
    }
}
