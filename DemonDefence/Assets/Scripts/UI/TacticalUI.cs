using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TacticalUI : MonoBehaviour
{

    public static TacticalUI Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    private TextElement diceText;
    private VisualElement rollDisplay;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(GenerateTurnUI());
        GameManager.OnGameStateChanged += GameManagerStateChanged;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(GenerateTurnUI("Default"));
    }
    private IEnumerator GenerateTurnUI(string faction = null)
    {
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

            var diceCard = Create("roll-card", faction.ToLower());
            diceCard.Add(diceText);
            rollDisplay.Add(diceCard);
        }


        container.Add(turnDisplay);

        root.Add(container);

        root.Add(rollDisplay);
        rollDisplay.style.display = DisplayStyle.None;

    }

    private IEnumerator GenerateEndUI(string faction = null)
    {
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
    public void setCardText(string text = null)
    {
        if(text == null) rollDisplay.style.display = DisplayStyle.None;
        else rollDisplay.style.display = DisplayStyle.Flex;
        diceText.text = text;
    }


    private void GameManagerStateChanged(GameState state)
    {
        switch (state)
        {
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
        return Create<VisualElement>(classNames);
    }

    private T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        var ele = new T();
        foreach (var className in classNames)
        {
            ele.AddToClassList(className);
        }

        return ele;
    }
}
