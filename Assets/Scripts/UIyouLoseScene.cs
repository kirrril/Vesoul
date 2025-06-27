using UnityEngine;
using UnityEngine.UIElements;

public class UIyouLoseScene : MonoBehaviour
{
    [SerializeField]
    private YouLoseSceneManager youLoseSceneManager;
    private UIDocument youLoseUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button stopButton;
    private ProgressBar remainingFuel;
    private VisualElement background;
    private Label comment;

    private StartStop startStop;

    void Awake()
    {
        startStop = new StartStop();

        youLoseUIdocument = GetComponent<UIDocument>();
        root = youLoseUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        stopButton = root.Q<Button>("stopButton");
        background = root.Q<VisualElement>("background");
        comment = root.Q<Label>("commentMessage");
        remainingFuel = root.Q<ProgressBar>("remainingFuel");

        youLoseSceneManager.getNowhereView += DisplayViewAndGreeting;
    }

    void OnEnable()
    {
        startButton.RegisterCallback<ClickEvent>(evt => startStop.HandleStartClick());
        stopButton.RegisterCallback<ClickEvent>(evt => startStop.HandleStopClick());
        remainingFuel.value = 0f;
    }

    void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(evt => startStop.HandleStartClick());
        stopButton.UnregisterCallback<ClickEvent>(evt => startStop.HandleStopClick());
    }

    private void DisplayViewAndGreeting(Texture2D nowhereView)
    {
        Sprite sprite = Sprite.Create(nowhereView, new Rect(0, 0, nowhereView.width, nowhereView.height), Vector2.one * 0.5f);
        background.style.backgroundImage = new StyleBackground(sprite);
        comment.text = $"VOUS N'AVEZ PLUS DE CARBURANT\nAPPUYEZ SUR START GAME\nPOUR CONTINUER LE VOYAGE";
    }
}
