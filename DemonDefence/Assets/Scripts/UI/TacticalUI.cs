using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TacticalUI : MonoBehaviour
{
    /// <summary>
    /// Control all UI functionality for the Tactical UI
    /// </summary>
    public static TacticalUI Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    private Button startButton;
    private Button skipButton;
    public bool mouseOnUI;
    public bool generated;
    VisualElement root => _document.rootVisualElement;
    VisualElement turnDisplay => root.Q<VisualElement>(className: "turn-display");
    VisualElement rollDisplay => root.Q<VisualElement>(className: "result-cards");
    VisualElement actionDisplay => root.Q<VisualElement>(className: "actions");

    private void Awake()
    {
        Instance = this;
        StartCoroutine(GenerateInstructionUI());
        GameManager.OnGameStateChanged += GameManagerStateChanged;

        startButton = Create("Start", startGame, "start-button");

        skipButton = Create("End\nTurn", endTurn, "skip-button", "player");
        generated = false;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        generated = false;

        skipButton = Create("End\nTurn", endTurn, "skip-button", "player");
        StartCoroutine(GenerateTurnUI());
        StartCoroutine(PopulateTurnUI("Default"));

    }

    private IEnumerator GenerateInstructionUI()
    {
        /// Generate the instruction UI
        TextAsset mytxtData = (TextAsset)Resources.Load("instructions");
        var txt = mytxtData.text.Split("\n");
        Debug.Log($"Generate instruction UI");
        yield return null;
        var root = _document.rootVisualElement;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container", "text-block");

        ScrollView textBlock = new ScrollView(ScrollViewMode.Vertical);
        textBlock.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        textBlock.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        Label header = Create<Label>("header-text");
        header.text = "Hell Broke Loose";
        textBlock.Add(header);
        textBlock.AddToClassList("unity-scroll-view__content-container");

        foreach (string line in txt)
        {
            var instruction = Create<TextElement>("instructions");
            instruction.text = line;
            textBlock.Add(instruction);
        }


        container.Add(textBlock);
        container.Add(startButton);
        root.Add(container);

    }

    private void startGame()
    {
        /// Start the game
        GameManager.Instance.UpdateGameState(GameState.CreateGrid);
    }

    private void endTurn()
    {
        Debug.Log("end turn");
        if (GameManager.Instance.State != GameState.PlayerTurn) return;
        UnitManager.Instance.SetSelectedHero(null);
        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }
    private IEnumerator GenerateTurnUI()
    {
        /// Generate the Turn UI
        /// Args:
        ///     string faction: The faction whose turn to generate. Default null.
        Debug.Log($"Generate turn UI");
        yield return null;
        root.Clear();
        mouseOnUI = false;

        root.styleSheets.Add(_styleSheet);

        var container = Create("container");

        var rollDisplay = Create("roll-board", "result-cards");
        var actionDisplay = Create("roll-board","actions");

        container.Add(Create("turn-display"));

        root.Add(container);

        root.Add(rollDisplay);
        rollDisplay.style.display = DisplayStyle.None;
        root.Add(actionDisplay);
        actionDisplay.style.display = DisplayStyle.None;
        root.Add(skipButton);
        generated = true;
    }

    private IEnumerator PopulateTurnUI(string faction = null)
    {

        Debug.Log($"Populate {faction} UI");
        while(!generated)
            yield return null;

        turnDisplay.Clear();
        actionDisplay.Clear();
        rollDisplay.Clear();

        if (faction != null)
        {
            var turnTextBox = Create<TextElement>("turn-text-box", faction.ToLower());
            turnTextBox.text = $"{faction} Turn";
            turnDisplay.Add(turnTextBox);
        }

        if (faction.ToLower() == "player") UnitManager.Instance.setNextPlayer();
    }

    public void addAction(string buttonText, Action method)
    {
        Button btn = Create(buttonText, method, "player", "action-button");
        Debug.Log(actionDisplay);
        actionDisplay.style.display = DisplayStyle.Flex;
        actionDisplay.Add(btn);

        
    }

    public void clearActions()
    {
        mouseOnUI = false;
        actionDisplay.Clear();
    }
    private IEnumerator GenerateEndUI(string faction = null)
    {
        /// Generates the end UI
        /// Args:
        ///     string faction: The victorious faction, default null
        Debug.Log($"Generate {faction} victory");
        yield return null;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container");

        var resultDisplay = Create("turn-display");

        if (faction != null)
        {
            var result = Create<TextElement>("turn-text-box", faction.ToLower());
            resultDisplay.Add(result); 
            if (faction == "Player")
            {
                result.text = "Victory";
            }
            else if (faction == "Enemy")
            {
                result.text = "Defeat";
            }
        }
        container.Add(resultDisplay);

        root.Add(container);

    }

    public void DisplayResults(int[] results, Faction faction)
    {
        /// Display a set of dice results
        /// Args:
        ///     int[] results: The results to display
        
        for (int index = 0; index < results.Length; index++)
        {
            var diceCard = Create("roll-card", faction.ToString().ToLower());
            var diceCardText = Create<TextElement>("roll-unit");
            diceCardText.text = $"{results[index]}";
            diceCard.Add(diceCardText);
            rollDisplay.Add(diceCard);
        }
        rollDisplay.style.display = DisplayStyle.Flex;
    }
    public void ClearResults()
    {
        /// Clear dice results from the screen
        rollDisplay.Clear();
        rollDisplay.style.display = DisplayStyle.None;
    }
    private void GameManagerStateChanged(GameState state)
    {
        /// Call UI generation on game state change
        switch (state)
        {
            case GameState.InstructionPage:
                break;
            case GameState.CreateGrid:
                StartCoroutine(GenerateTurnUI());
                break;
            case GameState.SpawnPlayer:
                break;
            case GameState.SpawnEnemy:
                break;
            case GameState.PlayerTurn:
                StartCoroutine(PopulateTurnUI("Player"));
                enableSkip();
                break;
            case GameState.EnemyTurn:
                StartCoroutine(PopulateTurnUI("Enemy"));
                disableSkip();
                break;
            case GameState.Victory:
                StartCoroutine(GenerateEndUI("Player"));
                disableSkip();
                break;
            case GameState.Defeat:
                StartCoroutine(GenerateEndUI("Enemy"));
                disableSkip();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private Button Create(string buttonText, Action methodToCall, params string[] classNames)
    {
        /// Create a button element
        /// Args:
        ///     string buttonText: The text for the button
        ///     Action methodToCall: The method to attach to the button
        ///     params string[] classNames: List of class names
        /// Returns:
        ///     Button with the given classes, text and method
        Button btn = Create<Button>(classNames);
        btn.text = buttonText;
        btn.RegisterCallback<MouseUpEvent>((evt) => methodToCall());
        btn.RegisterCallback<PointerOverEvent>((evt) => mouseOnUI = true);
        btn.RegisterCallback<PointerOutEvent>((evt) => mouseOnUI = false);
        return btn;
    }
    private VisualElement Create(params string[] classNames)
    {
        /// Create a visual element
        /// Args:
        ///     params string[] classNames: List of class names
        /// Returns:
        ///     VisualElement with the given classes
        return Create<VisualElement>(classNames);
    }

    private T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        /// Create a UI element
        /// Args:
        ///     params string[] classNames: List of class names
        /// Returns:
        ///     Element of type T with the given classes
        var ele = new T();
        foreach (var className in classNames)
        {
            ele.AddToClassList(className);
        }

        return ele;
    }

    public void enableSkip()
    {
        skipButton.SetEnabled(true);
        skipButton.style.display = DisplayStyle.Flex;
    }

    public void disableSkip()
    {
        skipButton.SetEnabled(false);
        skipButton.style.display = DisplayStyle.None;
    }

    public void enableOrders()
    {
        VisualElement display = root.Q<VisualElement>(className: "actions");
        display.style.display = DisplayStyle.Flex;
    }

    public void disableOrders()
    {
        VisualElement display = root.Q<VisualElement>(className: "actions");
        display.style.display = DisplayStyle.None;
    }
}
