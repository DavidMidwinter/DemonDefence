using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    VisualElement root => _document.rootVisualElement;
    public int pageNumber;
    public List<VisualElement> pages;
    VisualElement pageDisplay => root.Q<VisualElement>(className: "instruction-pages");

    private void Awake()
    {
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;

    }

    public IEnumerator GenerateInstructionUI()
    {
        /// Generate the instruction UI
        TextAsset mytxtData = (TextAsset)Resources.Load("instructionpages");
        List<Texture2D> images = new List<Texture2D>(Resources.LoadAll<Texture2D>(Path.Combine("Images", "instructions")));
        var txt = mytxtData.text.Split("- ");
        Debug.Log($"Generate instruction UI");
        yield return null;
        var root = _document.rootVisualElement;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container", "text-block");

        ScrollView textBlock = new ScrollView(ScrollViewMode.Vertical);
        textBlock.AddToClassList("instruction-pages");
        textBlock.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        textBlock.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        Label header = Create<Label>("header-text");
        header.text = "Hell Broke Loose";
        container.Add(header);
        textBlock.AddToClassList("unity-scroll-view__content-container");
        
        

        for(int i = 0; i < txt.Length; i++)
        {
            var page = Create("page", "white-border");
            if(i < images.Count)
            {
                VisualElement imgDisplay = Create("image-display");
                Image img = Create<Image>("instruction-image");
                img.scaleMode = ScaleMode.ScaleToFit;
                img.image = images[i];
                imgDisplay.Add(img);
                page.Add(imgDisplay);
            }

            var instruction = Create<TextElement>("instructions");
            instruction.text = txt[i];
            page.Add(instruction);
            pages.Add(page);
        }
        Debug.Log(pages.Count);
        container.Add(textBlock);
        Button startButton = Create("Start", startGame, "start-button");
        Button prevPage = Create("Previous", loadPreviousPage, "start-button");
        Button nextPage = Create("Next", loadNextPage, "start-button");
        VisualElement buttons = Create("buttons");
        buttons.Add(prevPage);
        buttons.Add(startButton);
        buttons.Add(nextPage);
        container.Add(buttons);
        root.Add(container);
        loadPage();

    }

    private void startGame()
    {
        /// Start the game
        GameManager.Instance.UpdateGameState(GameState.CreateGrid);
    }

    private void loadPage()
    {
        Debug.Log(pageNumber);
        pageDisplay.Clear();
        pageDisplay.Add(pages[pageNumber]);
    }
    private void loadNextPage()
    {
        if (pageNumber >= pages.Count - 1)
            return;

        pageNumber++;
        loadPage();
    }

    private void loadPreviousPage()
    {
        Debug.Log(pageNumber);
        if (pageNumber <= 0)
            return;

        pageNumber--;
        loadPage();
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
