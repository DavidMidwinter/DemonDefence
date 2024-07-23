using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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

    VisualElement detachmentPage;
    VisualElement pageDisplay => root.Q<VisualElement>(className: "instruction-pages");

    VisualElement citySettings => root.Q<VisualElement>(className: "city-settings");

    Button startButton => root.Q<Button>(className: "start-button");

    private List<string> cityLookups = new List<string> { "set-city-size", "set-walled", "set-buildings" };

    private (string name, int min, int max, string lookup, int defaultvalue)[] slider_settings =
    {
        ("Number of Buildings (-1 for no limit)", -1, 100, "set-buildings", -1),
        ("Spawn Radius", 2, 5, "set-radius", 5),
        ("City Size", 10, 25, "set-city-size", 12),
        ("Grid Size", 10, 70, "set-grid-size", 50),
        ("Trees %", 0, 20, "set-tree-chance", 10),
        ("Bushes %", 0, 20, "set-bush-chance", 10),
    };

    private (string name, int min, int max, string lookup, int defaultvalue)[] player_units =
    {
        ("Spearman Detachments", 0, 5, "set-spearmen", 1),
        ("Musket Detachments", 0, 5, "set-muskets", 1),
        ("Field Gun Detachments", 0, 5, "set-field-guns", 1),
    };

    private (string name, int min, int max, string lookup, int defaultvalue)[] enemy_units =
    {
        ("Demon Detachments", 0, 5, "set-demons", 1),
        ("Kite Detachments", 0, 5, "set-kites", 1),
    };

    private (string name, string lookup, string defaultValue)[] text_settings =
    {
        ("Map (blank for random)","set-map-name", ""),
    }; 
    private (string name, string lookup, bool defaultValue)[] bool_settings =
     {
        ("City","set-city-exists", true),
        ("Walled","set-walled", true),
        ("Night","set-night", false),
    };

    private void Awake()
    {
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;
        StartCoroutine(GenerateInstructionUI());


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return; 
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;
        StartCoroutine(GenerateInstructionUI(-2));
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

        VisualElement textBlock = Create("instruction-pages");

        Label header = Create<Label>("header-text");
        header.text = "Hell Broke Loose";
        container.Add(header);
        createPages(txt, images);
        Debug.Log(pages.Count);
        createDetachmentPage(txt.Length + 2);
        pages.Add(detachmentPage);
        createGameSettingsPage(txt.Length + 2);
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
            var page_number = createPageNumberDisplay(i + 1, txt.Length + 2);
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
        /// 
        SceneManager.LoadScene("Tactical");
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

        VisualElement playerUnits = Create("setting-display", "white-border", "player"); 
        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in player_units)
            playerUnits.Add(createSettingSlider(
                setting, null
                ));

        VisualElement enemyUnits = Create("setting-display", "white-border", "enemy");
        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in enemy_units)
            enemyUnits.Add(createSettingSlider(
                setting, null
                ));

        
        settingsBlock.Add(playerUnits);
        settingsBlock.Add(enemyUnits);
        VisualElement citySettingsBlock = Create("setting-display-double", "white-border", "city-settings");
        VisualElement generalSettings = Create("setting-display-double");

        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in slider_settings) {
            if (cityLookups.Contains(setting.lookup)){
                citySettingsBlock.Add(createSettingSlider(setting));
            }
            else
                generalSettings.Add(createSettingSlider(setting));
        }
        foreach ((string name, string lookup, string defaultValue) setting in text_settings)
            generalSettings.Add(createSettingTextbox(
                setting
                ));
        foreach ((string name, string lookup, bool defaultValue) setting in bool_settings)
        {
            if (cityLookups.Contains(setting.lookup))
            {
                citySettingsBlock.Add(createSettingCheckbox(setting));
            }
            else if(setting.lookup == "set-city-exists")
                settingsBlock.Add(createSettingCheckbox(setting));
            else
                generalSettings.Add(createSettingCheckbox(setting));
        }
        settingsBlock.Add(citySettingsBlock);
        settingsBlock.Add(generalSettings);
        gameSettings.Add(settingsBlock);
        
    }

    public void createDetachmentPage(int numberOfPages)
    {
        detachmentPage = Create("page", "white-border", "detachments");
        Label header = Create<Label>("header-text");
        header.text = "Detachments";
        var page_number = createPageNumberDisplay(numberOfPages - 1, numberOfPages);

        detachmentPage.Add(header);
        detachmentPage.Add(page_number);

        VisualElement detachmentBlock = Create("detachment-page");

        ScrollView playerUnits = new ScrollView(ScrollViewMode.Vertical);
        playerUnits.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        playerUnits.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        playerUnits.AddToClassList("unity-scroll-view__content-container");
        playerUnits.AddToClassList("detachment-block");
        playerUnits.AddToClassList("player");

        ScrollView enemyUnits = new ScrollView(ScrollViewMode.Vertical);
        enemyUnits.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        enemyUnits.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        enemyUnits.AddToClassList("unity-scroll-view__content-container");
        enemyUnits.AddToClassList("detachment-block");
        enemyUnits.AddToClassList("enemy");

        Label playerHeader = Create<Label>("header-text");
        playerHeader.text = "Player";
        playerUnits.Add(playerHeader);

        Label enemyHeader = Create<Label>("header-text");
        enemyHeader.text = "Enemy";
        enemyUnits.Add(enemyHeader);
        foreach (ScriptableDetachment detachment in Resources.LoadAll<ScriptableDetachment>("Detachments"))
        {
            if (detachment.Faction == Faction.Player)
                playerUnits.Add(createDetachmentCard(detachment));
            else if (detachment.Faction == Faction.Enemy)
                enemyUnits.Add(createDetachmentCard(detachment));
        }
        detachmentBlock.Add(playerUnits);
        detachmentBlock.Add(enemyUnits);

        detachmentPage.Add(detachmentBlock);
    }

    public VisualElement createDetachmentCard(ScriptableDetachment detachment)
    {
        VisualElement card = Create("white-border", "detachment-card");

        Label detachmentName = Create<Label>("detachment-header-text");
        detachmentName.text = detachment.unitName;
        card.Add(detachmentName); 
        Label detachmentSize = Create<Label>("detachment-troop-number-text");
        detachmentSize.text = $"Number of Troops: {detachment.numberOfTroops}";
        card.Add(detachmentSize);

        VisualElement leaderCard = createUnitCard(detachment.leaderUnit, detachment.leaderImage);
        VisualElement unitCard = createUnitCard(detachment.troopUnit, detachment.troopImage);

        card.Add(leaderCard);
        Label leaderAbility = Create<Label>("instruction-text", "unit-info-text");
        leaderAbility.text = string.Join("\n",detachment.leaderAbilities);
        card.Add(leaderAbility);

        card.Add(unitCard);

        return card;
    }

    public VisualElement createUnitCard(BaseUnit unit, Texture2D unitimg)
    {
        VisualElement card = Create("unit-card");
        VisualElement mainInfo = Create("unit-main-body");
        VisualElement imageDisplay = Create("unit-info");
        Image img = Create<Image>("unit-image");
        img.image = unitimg;
        imageDisplay.Add(img);

        VisualElement statsDisplay1 = Create("unit-info");
        Label unitStats = Create<Label>("instruction-text", "unit-info-text");
        unitStats.text =
            $"Movement: {unit.maxMovement}\n" +
            $"Members: {unit.individuals.Count}\n" +
            $"Health: {unit.individualHealth} / {unit.individualHealth * unit.individuals.Count}\n" +
            $"Toughness: {unit.toughness}";
        Label name = Create<Label>("unit-name");
        name.text = unit.name;

        statsDisplay1.Add(name);
        statsDisplay1.Add(unitStats);

        VisualElement statsDisplay2 = Create("unit-info");
        Label weaponStats = Create<Label>("instruction-text", "unit-info-text");
        weaponStats.text =
            $"Range: {unit.minimumRange} - {unit.maximumRange}\n" +
            $"Strength: {unit.strength}\n" +
            $"Damage: {unit.attackDamage}\n" +
            $"Actions: {unit.maxActions}";

        if(unit.attackActionsRequired)
        {
            weaponStats.text += $" [required to attack]";
        }

        Label blank = Create<Label>("unit-name");
        statsDisplay2.Add(blank);
        statsDisplay2.Add(weaponStats);

        mainInfo.Add(imageDisplay);
        mainInfo.Add(statsDisplay1);
        mainInfo.Add(statsDisplay2);

        card.Add(mainInfo);


        Label types = Create<Label>("instruction-text","unit-info-text");
        types.text = $"Unit Types: {string.Join(", ", unit.unitTypes)}\n" +
            $"Strong Against: {string.Join(", ", unit.strongAgainst)}\n" +
            $"Weak Against: {string.Join(", ", unit.weakAgainst)}";
        card.Add(types);

        return card;
    }

    public VisualElement createSettingSlider((string name, int minimum, int maximum, string lookup, int defaultValue) setting, string displayclass = "setting-display")
    {
        /// Create a slider-based setting.
        /// Args:
        ///     (string name, int minimum, int maximum, string lookup, int defaultValue) setting: The setting to make a slider
        ///     for.
        VisualElement settingDisplay = Create(displayclass);
        Label settingName = Create<Label>("instruction-text");
        settingName.text = setting.name;
        settingDisplay.Add(settingName);
        SliderInt slider = Create<SliderInt>(setting.lookup);
        slider.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        slider.lowValue = setting.minimum;
        slider.highValue = setting.maximum;
        slider.value = setting.defaultValue;
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
        setValue(setting.lookup, setting.defaultValue);
        settingDisplay.Add(settingName);
        settingDisplay.Add(text);
        return settingDisplay;
    }
    public VisualElement createSettingCheckbox((string name, string lookup, bool defaultValue) setting)
    {
        VisualElement settingDisplay = Create("setting-display");
        Label settingName = Create<Label>("instruction-text");
        settingName.text = setting.name;

        Toggle text = Create<Toggle>(setting.lookup);
        text.value = setting.defaultValue;
        text.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        setValue(setting.lookup, setting.defaultValue);
        settingDisplay.Add(settingName);
        settingDisplay.Add(text);
        return settingDisplay;
    }
    private void setValue(string lookup, int value)
    {
        Debug.Log(lookup);
        switch (lookup)
        {
            case ("set-grid-size"):
                alignCitySizewithGridRange(value);
                break;
            case "set-city-size":
                alignWalledToggleWithRanges(value);
                break;
            default:
                break;
        }
        TacticalStartData.setGameSettingValues(lookup, value);
    }

    private void alignCitySizewithGridRange(int value)
    {
        SliderInt citySize = root.Q<SliderInt>(className: "set-city-size");
        if (citySize != null)
        {
            citySize.lowValue = value > 40 ? value / 4 : 10;
            citySize.highValue = value > 20 ? value / 2 : 10;

            if (citySize.value < citySize.lowValue) citySize.value = citySize.lowValue;
            else if (citySize.value > citySize.highValue) citySize.value = citySize.highValue;
            else setValue("set-city-size", citySize.value);
        }
    }

    private void alignWalledToggleWithRanges(int value)
    {
        if (!Application.isPlaying) return;
        SliderInt gridSize = root.Q<SliderInt>(className: "set-grid-size");
        Toggle walled = root.Q<Toggle>(className: "set-walled");
        if (walled != null && gridSize != null)
        {
            if ((value * 2) + 5 >= gridSize.value - 1)
            {
                walled.value = false;
                walled.SetEnabled(false);
            }
            else
            {
                walled.SetEnabled(true);
            }
        }
    }
    private void setValue(string lookup, string value)
    {
        if (!Application.isPlaying) return;
        TacticalStartData.setGameSettingValues(lookup, value);
    }
    private void setValue(string lookup, bool value)
    {
        if (!Application.isPlaying) return;
        TacticalStartData.setGameSettingValues(lookup, value);


        if (citySettings == null) return;
        if(lookup == "set-city-exists")
        {
            if (value)
            {
                citySettings.SetEnabled(true);
                citySettings.style.display = DisplayStyle.Flex;
            }
            else
            {
                citySettings.SetEnabled(false);
                citySettings.style.display = DisplayStyle.None;
            }
        }
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