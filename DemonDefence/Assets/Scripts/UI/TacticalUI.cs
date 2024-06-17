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
    private TextElement diceText;
    private List<TextElement> cards;
    private VisualElement rollDisplay;
    private Button startButton;
    private Button skipButton;
    [SerializeField] private int cardNumber;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(GenerateInstructionUI());
        GameManager.OnGameStateChanged += GameManagerStateChanged;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(GenerateTurnUI("Default"));
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

        startButton = Create<Button>("start-button");
        startButton.text = "Start";
        startButton.RegisterCallback<MouseUpEvent>((evt) => startGame());

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
        UnitManager.Instance.SetSelectedHero(null);
        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }
    private IEnumerator GenerateTurnUI(string faction = null)
    {
        /// Generate the Turn UI
        /// Args:
        ///     string faction: The faction whose turn to generate. Default null.
        Debug.Log($"Generate {faction} UI");
        yield return null;
        var root = _document.rootVisualElement;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container");

        var turnDisplay = Create("turn-display");

        rollDisplay = Create("roll-board");

        diceText = Create<TextElement>("roll-unit");
        setCardText();

        if (faction != null)
        {
            var turnTextBox = Create<TextElement>("turn-text-box", faction.ToLower());
            turnTextBox.text = $"{faction} Turn";
            turnDisplay.Add(turnTextBox);

            skipButton = Create<Button>("start-button");
            skipButton.text = "End Turn";
            skipButton.RegisterCallback<MouseUpEvent>((evt) => endTurn());

            cards = new List<TextElement>();
            for (int i = 0; i < cardNumber; i++) {

                var diceCard = Create("roll-card", faction.ToLower());
                var diceCardText = Create<TextElement>("roll-unit");
                diceCard.Add(diceCardText);
                diceCard.style.display = DisplayStyle.None;
                cards.Add(diceCardText);
                rollDisplay.Add(diceCard); 
            }
        }


        container.Add(turnDisplay);

        root.Add(container);

        root.Add(rollDisplay);
        rollDisplay.style.display = DisplayStyle.None;

        if (faction.ToLower() == "player") container.Add(skipButton);

    }

    private IEnumerator GenerateEndUI(string faction = null)
    {
        /// Generates the end UI
        /// Args:
        ///     string faction: The victorious faction, default null
        Debug.Log($"Generate {faction} victory");
        yield return null;
        var root = _document.rootVisualElement;
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

    public void DisplayResults(int[] results)
    {
        /// Display a set of dice results
        /// Args:
        ///     int[] results: The results to display
        for (int index = 0; index < results.Length; index++)
        {
            if(index < cards.Count)
            {
                cards[index].text = $" {results[index]} ";
                cards[index].parent.style.display = DisplayStyle.Flex;
            }
        }
        rollDisplay.style.display = DisplayStyle.Flex;
    }
    public void ClearResults()
    {
        /// Clear dice results from the screen
        foreach(TextElement text in cards)
        {
            text.text = "";
            text.parent.style.display = DisplayStyle.None;
        }
        rollDisplay.style.display = DisplayStyle.None;
    }
    public void setCardText(string text = null)
    {
        /// Set text for a text card
        /// Args:
        ///     string text: Text to set, default null
        if(text == null) rollDisplay.style.display = DisplayStyle.None;
        else rollDisplay.style.display = DisplayStyle.Flex;
        diceText.text = text;
    }


    private void GameManagerStateChanged(GameState state)
    {
        /// Call UI generation on game state change
        switch (state)
        {
            case GameState.InstructionPage:
                break;
            case GameState.CreateGrid:
                break;
            case GameState.SpawnPlayer:
                break;
            case GameState.SpawnEnemy:
                break;
            case GameState.PlayerTurn:
                StartCoroutine(GenerateTurnUI("Player"));
                break;
            case GameState.EnemyTurn:
                StartCoroutine(GenerateTurnUI("Enemy"));
                break;
            case GameState.Victory:
                StartCoroutine(GenerateEndUI("Player"));
                break;
            case GameState.Defeat:
                StartCoroutine(GenerateEndUI("Enemy"));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
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
}
