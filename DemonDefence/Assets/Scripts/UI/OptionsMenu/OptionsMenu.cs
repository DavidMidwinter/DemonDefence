using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class OptionsMenu
{
    static VisualElement gameOptions;

    public static VisualElement getOptionsMenu(Button backButton)
    {
        if(gameOptions is null)
        {
            createOptionsMenu();
        }
        VisualElement menu = UITools.Create();
        menu.Add(gameOptions);
        menu.Add(backButton);
        return menu;
    }

    static void createOptionsMenu()
    {
        gameOptions = UITools.Create();
        VisualElement volumeSetting = createSettingSlider(("Volume", 0, 100, "volume"));
        gameOptions.Add(volumeSetting);
        VisualElement playbackSetting = createSettingSlider(("Playback Speed %", 50, 200, "playback-speed"));
        gameOptions.Add(playbackSetting);
    }

    public static VisualElement createSettingSlider((string name, int minimum, int maximum, string lookup) setting, string displayclass = "setting-display")
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
        slider.value = PlayerSettings.getPref(setting.lookup);
        slider.showInputField = true;
        settingDisplay.Add(slider);
        return settingDisplay;
    }

    private static void setValue(string key, int value)
    {
        Debug.Log($"[OptionsMenu]: Set value {key}: {value}");

        PlayerSettings.setPref(key, value);
    }
}

