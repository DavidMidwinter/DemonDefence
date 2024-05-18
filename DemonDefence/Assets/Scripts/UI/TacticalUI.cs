using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TacticalUI : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;

    private void Awake()
    {
        StartCoroutine(Generate());
        GameManager.OnGameStateChanged += GameManagerStateChanged;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(Generate("Default"));
    }
    private IEnumerator Generate(string faction = null)
    {
        Debug.Log($"Generate {faction}");
        yield return null;
        var root = _document.rootVisualElement;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container");

        var turnDisplay = Create("turn-display");

        if (faction != null)
        {
            var turnTextBox = Create<TextElement>("turn-text-box", faction.ToLower());
            turnTextBox.text = $"{faction} Turn";
            turnDisplay.Add(turnTextBox);
        }

        container.Add(turnDisplay);

        root.Add(container);
        var rollDisplay = Create("roll-board");
        var diceText = Create<TextElement>("roll-unit");
        diceText.text = "10";
        rollDisplay.Add(diceText);
        //root.Add(rollDisplay);

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
                StartCoroutine(Generate("Player"));
                break;
            case GameState.EnemyTurn:
                StartCoroutine(Generate("Enemy"));
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
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
