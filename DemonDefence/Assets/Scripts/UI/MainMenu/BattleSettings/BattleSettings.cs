using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public static class BattleSettings
{
    static VisualElement gameSettings; 
    static Button startButton => gameSettings.Q<Button>(className: "start-button");
    public static int maxPlayerDetachments = 6, maxEnemyDetachments = 6;
    public static int numberOfPlayerDetachments = 0, numberOfEnemyDetachments = 0;

    static VisualElement mapGeneration;

    static VisualElement mapLoad;

    static VisualElement citySettings => mapGeneration.Q<VisualElement>(className: "city-settings");

    static Label playerDetachmentNumberDisplay => gameSettings.Q<Label>(className: "player-detachments");
    static Label enemyDetachmentNumberDisplay => gameSettings.Q<Label>(className: "enemy-detachments");
    static Label playerDetachmentWarning => gameSettings.Q<Label>(className: "player-detachments-warning");
    static Label enemyDetachmentWarning => gameSettings.Q<Label>(className: "enemy-detachments-warning");
    private static List<string> playerLookups, enemyLookups;

    private static (string name, int min, int max, string lookup, int defaultvalue)[] player_units =
    {
        (DetachmentData.SPEARMEN, 0, 5, "set-spearmen", 1),
        (DetachmentData.TEMPLARS, 0, 5, "set-templars", 1),
        (DetachmentData.MUSKETS, 0, 5, "set-muskets", 1),
        (DetachmentData.FIELD_GUNS, 0, 2, "set-field-guns", 1),
        (DetachmentData.ORGAN_GUNS, 0, 2, "set-organ-guns", 1),
    };

    private static (string name, int min, int max, string lookup, int defaultvalue)[] enemy_units =
    {
        (DetachmentData.CULTISTS, 0, 5, "set-cultists", 1),
        (DetachmentData.HELLSPAWN, 0, 5, "set-hellspawn", 1),
        (DetachmentData.DEMONS, 0, 5, "set-demons", 1),
        (DetachmentData.KITES, 0, 5, "set-kites", 1),
        (DetachmentData.INFERNAL_ENGINES, 0, 2, "set-infernal-engines", 1),
    };

    private static (string name, string lookup, string defaultValue)[] text_settings =
    {
        ("Map (blank for random)","set-map-name", ""),
    };
    private static (string name, string lookup, bool defaultValue)[] bool_settings =
     {
        ("City","set-city-exists", true),
        ("Walled","set-walled", true),
        ("Night - enemy goes first","set-night", false)
    };

    public static VisualElement getBattleSettingsPage(bool forceGenerate = false)
    {
        if (forceGenerate || gameSettings is null)
        {
            createGameSettingsPage();
        }

        mapGeneration = MapGeneration.getMapGenerationPage(forceGenerate);

        mapLoad = MapLoad.getMapLoadPage(forceGenerate);

        return gameSettings;
    }
    public static void createGameSettingsPage()
    {
        /// Create the settings page
        /// Args:
        ///     int numberOfPages: The total number of pages; settings will always be the last page.
        ///     

        playerLookups = new List<string>();
        enemyLookups = new List<string>();

        gameSettings = UITools.Create("page", "white-border");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Game Settings";

        gameSettings.Add(header);


        ScrollView settingsBlock = UITools.Create(ScrollViewMode.Vertical, "settings-block");

        Button loadMapGenPage = UITools.Create("Map Generation Settings", loadMapGeneration, "setting-button", "white-border", "map-gen-page");
        settingsBlock.Add(loadMapGenPage);
        Button loadMapLoadPage = UITools.Create("Saved Maps", loadMapLoad, "setting-button", "white-border", "map-gen-page");
        settingsBlock.Add(loadMapLoadPage);

        VisualElement playerUnits = UITools.Create("setting-display", "white-border", "player");

        playerUnits.Add(createDetachmentNumberDisplay("player-detachments"));

        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in player_units)
        {
            playerUnits.Add(createDetachmentSlider(
                setting, null
                ));
            playerLookups.Add(setting.lookup);
        }

        VisualElement enemyUnits = UITools.Create("setting-display", "white-border", "enemy");

        enemyUnits.Add(createDetachmentNumberDisplay("enemy-detachments"));
        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in enemy_units)
        {
            enemyUnits.Add(createDetachmentSlider(
                setting, null
                ));

            enemyLookups.Add(setting.lookup);
        }


        settingsBlock.Add(playerUnits);
        settingsBlock.Add(enemyUnits);
        gameSettings.Add(settingsBlock);

        

        gameSettings.Add(createButtonDisplay());
        checkPlayerDetachments();
        checkEnemyDetachments();

    }

    private static VisualElement createDetachmentSlider((string name, int minimum, int maximum, string lookup, int defaultValue) setting, string displayclass = "setting-display")
    {
        ScriptableDetachment detachment = DetachmentData.getDetachment(setting.name);
        return createSettingSlider((detachment.unitName, setting.minimum, setting.maximum, setting.lookup, setting.defaultValue), displayclass);
    }

    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");

        Button startButton = UITools.Create("Start", startGame, "instruction-ui-button", "start-button");
        buttons.Add(startButton);
        buttons.Add(MainMenu.Instance.backButton());
        return buttons;
    }
    public static VisualElement createDetachmentNumberDisplay(string lookup)
    {
        VisualElement detachmentDisplay = UITools.Create("detachment-number-display");

        Label displayHeader = UITools.Create<Label>("instruction-text");
        displayHeader.text = "Detachments:";

        Label currentAmount = UITools.Create<Label>(lookup, "instruction-text");

        Label warning = UITools.Create<Label>("instruction-text", $"{lookup}-warning");
        int max = 0;
        if (lookup == "enemy-detachments") max = maxEnemyDetachments;
        if (lookup == "player-detachments") max = maxPlayerDetachments;

        warning.text = $"Amount must be between 0 and {max}!";
        warning.style.display = DisplayStyle.None;

        detachmentDisplay.Add(displayHeader);
        detachmentDisplay.Add(currentAmount);
        detachmentDisplay.Add(warning);

        return detachmentDisplay;
    }

    public static VisualElement createSettingSlider((string name, int minimum, int maximum, string lookup, int defaultValue) setting, string displayclass = "setting-display")
    {
        /// Create a slider-based setting.
        /// Args:
        ///     (string name, int minimum, int maximum, string lookup, int defaultValue) setting: The setting to make a slider
        ///     for.
        VisualElement settingDisplay = UITools.Create(displayclass);
        Label settingName = UITools.Create<Label>("instruction-text");
        settingName.text = setting.name;
        settingDisplay.Add(settingName);
        SliderInt slider = UITools.Create<SliderInt>(setting.lookup);
        slider.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        slider.lowValue = setting.minimum;
        slider.highValue = setting.maximum;
        slider.value = setting.defaultValue;
        setValue(setting.lookup, setting.defaultValue);
        slider.showInputField = true;
        settingDisplay.Add(slider);
        return settingDisplay;
    }

    public static VisualElement createSettingTextbox((string name, string lookup, string defaultValue) setting)
    {
        VisualElement settingDisplay = UITools.Create("setting-display");
        Label settingName = UITools.Create<Label>("instruction-text");
        settingName.text = setting.name;

        TextField text = UITools.Create<TextField>(setting.lookup);
        text.value = setting.defaultValue;
        text.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        setValue(setting.lookup, setting.defaultValue);
        settingDisplay.Add(settingName);
        settingDisplay.Add(text);
        return settingDisplay;
    }
    public static VisualElement createSettingCheckbox((string name, string lookup, bool defaultValue) setting)
    {
        VisualElement settingDisplay = UITools.Create("setting-display");
        Label settingName = UITools.Create<Label>("instruction-text");
        settingName.text = setting.name;

        Toggle text = UITools.Create<Toggle>(setting.lookup);
        text.value = setting.defaultValue;
        text.RegisterValueChangedCallback(evt => setValue(setting.lookup, evt.newValue));
        setValue(setting.lookup, setting.defaultValue);
        settingDisplay.Add(settingName);
        settingDisplay.Add(text);
        return settingDisplay;
    }

    private static void setPlayerDetachmentNumber()
    {
        playerDetachmentNumberDisplay.text = $"{numberOfPlayerDetachments}/{maxPlayerDetachments}";
        if (numberOfPlayerDetachments <= 0 || numberOfPlayerDetachments > maxPlayerDetachments) playerDetachmentWarning.style.display = DisplayStyle.Flex;
        else playerDetachmentWarning.style.display = DisplayStyle.None;
    }
    private static void setEnemyDetachmentNumber()
    {
        enemyDetachmentNumberDisplay.text = $"{numberOfEnemyDetachments}/{maxEnemyDetachments}";
        if (numberOfEnemyDetachments <= 0 || numberOfEnemyDetachments > maxEnemyDetachments) enemyDetachmentWarning.style.display = DisplayStyle.Flex;
        else enemyDetachmentWarning.style.display = DisplayStyle.None;
    }
    private static void setValue(string lookup, int value)
    {
        Debug.LogWarning(lookup);
        if (playerLookups.Contains(lookup)) checkPlayerDetachments();
        else if (enemyLookups.Contains(lookup)) checkEnemyDetachments();
        switch (lookup)
        {
            case "set-grid-size":
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
    private static void checkPlayerDetachments()
    {
        numberOfPlayerDetachments = 0;
        foreach (string lookup in playerLookups)
        {
            numberOfPlayerDetachments += gameSettings.Q<SliderInt>(className: lookup).value;
        }
        setPlayerDetachmentNumber();

        canStart();
    }
    private static void checkEnemyDetachments()
    {
        numberOfEnemyDetachments = 0;
        foreach (string lookup in enemyLookups)
        {
            numberOfEnemyDetachments += gameSettings.Q<SliderInt>(className: lookup).value;
        }
        setEnemyDetachmentNumber();

        canStart();
    }

    private static void alignCitySizewithGridRange(int value)
    {
        if (mapGeneration is null) return;
        SliderInt citySize = mapGeneration.Q<SliderInt>(className: "set-city-size");
        if (citySize != null)
        {
            citySize.lowValue = value > 40 ? value / 4 : 10;
            citySize.highValue = value > 20 ? value / 2 : 10;

            if (citySize.value < citySize.lowValue) citySize.value = citySize.lowValue;
            else if (citySize.value > citySize.highValue) citySize.value = citySize.highValue;
            else setValue("set-city-size", citySize.value);
        }
    }

    private static void alignWalledToggleWithRanges(int value)
    {
        if (!Application.isPlaying) return;
        if (mapGeneration is null) return;
        SliderInt gridSize = mapGeneration.Q<SliderInt>(className: "set-grid-size");
        Toggle walled = mapGeneration.Q<Toggle>(className: "set-walled");
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
    public static void setValue(string lookup, string value)
    {
        if (!Application.isPlaying) return;
        TacticalStartData.setGameSettingValues(lookup, value);
    }
    private static void setValue(string lookup, bool value)
    {
        if (!Application.isPlaying) return;
        TacticalStartData.setGameSettingValues(lookup, value);

        if (mapGeneration is null) return;
        if (citySettings == null) return;
        if (lookup == "set-city-exists")
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


    private static void startGame()
    {
        /// Start the game
        /// 
        SceneManager.LoadScene("Tactical");
    }

    public static void canStart()
    {
        try
        {

            if (numberOfEnemyDetachments <= 0
                    || numberOfEnemyDetachments > maxEnemyDetachments)
            {
                startButton.SetEnabled(false);
                return;
            }

            if (numberOfPlayerDetachments <= 0
                    || numberOfPlayerDetachments > maxPlayerDetachments)
            {
                startButton.SetEnabled(false);
                return;

            }

            startButton.SetEnabled(true);
        }
        catch (NullReferenceException)
        {
            return;
        }

    }

    private static void loadMapGeneration()
    {
        MainMenu.Instance.loadExternal(mapGeneration);
    }

    private static void loadMapLoad()
    {
        MainMenu.Instance.loadExternal(mapLoad);
    }

    public static Button backButton()
    {
        Button backButton = UITools.Create("Main Game Settings Menu", MainMenu.Instance.loadBattleUI, "back-button");
        return backButton;
    }
}
