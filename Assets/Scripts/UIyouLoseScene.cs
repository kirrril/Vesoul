using UnityEngine;
using UnityEngine.UIElements;

public class UIyouLoseScene : MonoBehaviour
{
    private UIDocument youLoseUIdocument;
    private VisualElement root;
    private Button startButton;
    private Button stopButton;

    private StartStop startStop;

    void Awake()
    {
        startStop = new StartStop();

        youLoseUIdocument = GetComponent<UIDocument>();
        root = youLoseUIdocument.rootVisualElement;
        startButton = root.Q<Button>("startButton");
        stopButton = root.Q<Button>("stopButton");

        startButton.SetEnabled(false);
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
}
