using UnityEngine.UIElements;

public static class MapNew
{
    static VisualElement mapNewPage;

    static Button newFileButton => (Button)mapNewPage.Q(className: "load-file-button");

    private static (string name, int min, int max, string lookup, int defaultvalue)[] slider_settings =
    {
        ("Grid Size", 10, 70, "set-grid-size", 50),
    };

    private static (string name, string lookup, string defaultValue)[] text_settings =
    {
        ("Map Name", "set-map-name", null)
    };

    public static VisualElement getMapNewPage(bool forceGenerate = false)
    {
        if (forceGenerate || mapNewPage is null)
        {
            createMapNewPage();
        }

        return mapNewPage;
    }

    public static void createMapNewPage()
    {

        mapNewPage = UITools.Create("page", "white-border");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Load Map";

        mapNewPage.Add(header);

        ScrollView settingsBlock = UITools.Create(ScrollViewMode.Vertical, "settings-block");
        foreach((string name, string lookup, string defaultValue) setting in text_settings)
        {
            settingsBlock.Add(createSettingTextbox(setting));
        }
        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in slider_settings)
        {
            settingsBlock.Add(createSettingSlider(setting));
        }

        mapNewPage.Add(settingsBlock);
        mapNewPage.Add(createButtonDisplay());

        newFileButton.SetEnabled(false);

    }

    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");
        buttons.Add(MainMenu.Instance.backButton());
        buttons.Add(MainMenu.Instance.loadEditorButton("New File"));
        return buttons;
    }
    private static void setValue(string lookup, string value)
    {
        MainMenu.Instance.setValue(lookup, value);
        if (lookup == "set-map-name" && newFileButton is not null)
        {
            if (value != null)
                newFileButton.SetEnabled(true);
            else
                newFileButton.SetEnabled(false);
        }
    }
    private static void setValue(string lookup, int value)
    {
        MainMenu.Instance.setValue(lookup, value);
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
}
