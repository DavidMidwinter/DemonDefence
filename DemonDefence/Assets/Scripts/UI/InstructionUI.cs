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

    VisualElement gameSettings;
    VisualElement pageDisplay => root.Q<VisualElement>(className: "instruction-pages");

    Button startButton => root.Q<Button>(className: "start-button");

    private (string name, int min, int max, string lookup, int defaultvalue)[] slider_settings =
    {
        ("Spearman Detachments", 0, 5, "set-spearmen", 3),
        ("Demon Detachments", 0, 5, "set-demons", 3),
        ("Number of Buildings (-1 for no limit)", -1, 100, "set-buildings", -1),
        ("Spawn Radius", 2, 5, "set-radius", 5),
        ("City Size", 10, 10, "set-city-size", 10),
        ("Grid Size", 10, 100, "set-grid-size", 20),
    };

    private (string name, string lookup, string defaultValue)[] text_settings =
    {
        ("Map (blank for random)","set-map-name", ""),
    };

    private void Awake()
    {
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return; 
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;
        StartCoroutine(GenerateInstructionUI(-1));
    }

    public IEnumerator GenerateInstructionUI(int startpage = 0)
    {
        /// Generate the instruction UI
        TextAsset mytxtData = (TextAsset)Resources.Load("instructionpages");
        List<Texture2D> images = new List<Texture2D>(Resources.LoadAll<Texture2D>(Path.Combine("Images", "instructions")));
        var txt = mytxtData.text.Split("- ");
        Debug.Log($"Generate instruction UI");
        yield return null;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = Create("container", "text-block");

        ScrollView textBlock = new ScrollView(ScrollViewMode.Vertical);
        textBlock.AddToClassList("instruction-pages");
        textBlock.verticalScrollerVisibility = ScrollerVisibility.Auto;
        textBlock.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        textBlock.AddToClassList("unity-scroll-view__content-container");

        Label header = Create<Label>("header-text");
        header.text = "Hell Broke Loose";
        container.Add(header);
        createPages(txt, images);
        Debug.Log(pages.Count);
        createGameSettingsPage(txt.Length+1);
        pages.Add(gameSettings);
        container.Add(textBlock);
        VisualElement buttons = createButtonDisplay();
        pageNumber = startpage >= 0 ? startpage : pages.Count + startpage;
        container.Add(buttons);
        root.Add(container);
        startButton.SetEnabled(false);
        loadPage();

    }

    private VisualElement createButtonDisplay()
    {
        VisualElement buttons = Create("buttons");
        Button startButton = Create("Start", startGame, "instruction-ui-button", "start-button");
        Button prevPage = Create("Previous", loadPreviousPage, "instruction-ui-button", "previous-button");
        Button nextPage = Create("Next", loadNextPage, "instruction-ui-button", "next-button");
        buttons.Add(prevPage);
        buttons.Add(startButton);
        buttons.Add(nextPage);
        return buttons;
    }
    private void createPages(string[] txt, List<Texture2D> images)
    {
        for (int i = 0; i < txt.Length; i++)
        {
            var page = Create("page", "white-border");
            if (i < images.Count)
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
            var page_number = createPageNumberDisplay(i + 1, txt.Length + 1);
            page.Add(page_number);
            pages.Add(page);
        }
    }

    private VisualElement createPageNumberDisplay(int pageNumber, int maxPages)
    {
        /// Creates a page number display
        /// Args:
        ///     int pageNumber: This page's Page Number
        ///     int maxPages: The total number of pages
        var page_number = Create<TextElement>("instructions", "page-number");
        page_number.text = $"{pageNumber}/{maxPages}";
        return page_number;
    }
    private void startGame()
    {
        /// Start the game
        GameManager.Instance.UpdateGameState(GameState.InitGame);
    }
    public void createGameSettingsPage(int numberOfPages)
    {
        /// Create the settings page
        /// Args:
        ///     int numberOfPages: The total number of pages; settings will always be the last page.
        gameSettings = Create("page", "white-border");
        Label header = Create<Label>("header-text");
        header.text = "Game Settings";
        var page_number = createPageNumberDisplay(numberOfPages,numberOfPages);

        gameSettings.Add(header);
        gameSettings.Add(page_number);

        ScrollView settingsBlock = new ScrollView(ScrollViewMode.Vertical);
        settingsBlock.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        settingsBlock.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        settingsBlock.AddToClassList("unity-scroll-view__content-container");
        settingsBlock.AddToClassList("settings-block");

        foreach ((string, int, int, string, int) setting in slider_settings)
            settingsBlock.Add(createSettingSlider(
                setting
                ));
        foreach ((string, string, string) setting in text_settings)
            settingsBlock.Add(createSettingTextbox(
                setting
                ));
        gameSettings.Add(settingsBlock);
        
    }

    public VisualElement createSettingSlider((string name, int minimum, int maximum, string lookup, int defaultValue) setting)
    {
        /// Create a slider-based setting.
        /// Args:
        ///     (string name, int minimum, int maximum, string lookup, int defaultValue) setting: The setting to make a slider
        ///     for.
        VisualElement settingDisplay = Create("setting-display");
        Label settingName = Create<Label>("instruction-text");
        settingName.text = setting.name;
        settingDisplay.Add(settingName);
        SliderInt slider = Create<SliderInt>(setting.lookup);
        slider.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        slider.lowValue = setting.minimum;
        slider.highValue = setting.maximum;
        slider.value = setting.defaultValue;
        if(GameManager.Instance)
            setValue(setting.lookup, setting.defaultValue);
        slider.showInputField = true;
        settingDisplay.Add(slider);
        return settingDisplay;
    }
    
    public VisualElement createSettingTextbox((string name, string lookup, string defaultValue) setting)
    {
        VisualElement settingDisplay = Create("setting-display");
        Label settingName = Create<Label>("instruction-text");
        settingName.text = setting.name;

        TextField text = Create<TextField>(setting.lookup);
        text.value = setting.defaultValue;
        text.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        if (GameManager.Instance)
            setValue(setting.lookup, setting.defaultValue);
        settingDisplay.Add(settingName);
        settingDisplay.Add(text);
        return settingDisplay;
    }
    private void setValue(string lookup, int value)
    {
        Debug.Log(lookup);
        if (lookup == "set-grid-size")
        {
            SliderInt citySize = root.Q<SliderInt>(className: "set-city-size");
            Debug.Log(citySize);
            if (citySize != null)
            {
                citySize.lowValue = value > 40 ? value / 4 : 10;
                citySize.highValue = value > 20 ? value / 2 : 10;
            }
        }
        GameManager.Instance.setGameSettingValues(lookup, value);
    }
    private void setValue(string lookup, string value)
    {
        GameManager.Instance.setGameSettingValues(lookup, value);
    }
    private void loadPage()
    {
        Debug.Log(pageNumber);
        pageDisplay.Clear();
        pageDisplay.Add(pages[pageNumber]);
        if (pageNumber == pages.Count - 1) startButton.SetEnabled(true);
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