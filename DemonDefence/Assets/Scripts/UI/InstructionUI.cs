using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InstructionUI : MonoBehaviour
{
    /// <summary>
    /// Control all UI functionality for the Tactical UI
    /// </summary>
    public static InstructionUI Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    private Button startButton;
    VisualElement root => _document.rootVisualElement;

    private void Awake()
    {
        Instance = this;
        StartCoroutine(GenerateInstructionUI());

        startButton = Create("Start", startGame, "start-button");


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;

    }

    public IEnumerator GenerateInstructionUI()
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
}
