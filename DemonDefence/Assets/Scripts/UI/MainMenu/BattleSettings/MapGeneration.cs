using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGeneration : MonoBehaviour
{
    static VisualElement mapGenerationPage;

    private static List<string> cityLookups = new List<string> { "set-city-size", "set-walled", "set-buildings" };

    private static (string name, int min, int max, string lookup, int defaultvalue)[] slider_settings =
    {
        ("Number of Buildings (-1 for no limit)", -1, 100, "set-buildings", -1),
        ("Spawn Radius", 2, 5, "set-radius", 5),
        ("City Size", 10, 25, "set-city-size", 12),
        ("Grid Size", 10, 70, "set-grid-size", 50),
        ("Trees %", 0, 20, "set-tree-chance", 10),
        ("Bushes %", 0, 20, "set-bush-chance", 10),
        ("Max Rivers", 0, 4, "set-river-number", 2),
        ("Player Spawn Areas", 1, 4, "set-player-spawn-number", 1),
        ("Enemy Spawn Areas", 1, 4, "set-enemy-spawn-number", 1),
    };
    private static (string name, string lookup, bool defaultValue)[] bool_settings =
     {
        ("City","set-city-exists", true),
        ("Walled","set-walled", true),
        ("Night - enemy goes first","set-night", false)
    };
    public static VisualElement getMapGenerationPage(bool forceGenerate = false)
    {
        if (forceGenerate || mapGenerationPage is null)
        {
            createMapGenerationPage();
        }

        return mapGenerationPage;
    }

    static void createMapGenerationPage()
    {

        mapGenerationPage = UITools.Create("page", "white-border");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Game Settings";

        mapGenerationPage.Add(header);



        ScrollView settingsBlock = UITools.Create(ScrollViewMode.Vertical, "settings-block");
        VisualElement citySettingsBlock = UITools.Create("setting-display-double", "white-border", "city-settings");
        VisualElement generalSettings = UITools.Create("setting-display-double");

        foreach ((string name, int min, int max, string lookup, int defaultvalue) setting in slider_settings)
        {
            if (cityLookups.Contains(setting.lookup))
            {
                citySettingsBlock.Add(BattleSettings.createSettingSlider(setting));
            }
            else
                generalSettings.Add(BattleSettings.createSettingSlider(setting));
        }
        foreach ((string name, string lookup, bool defaultValue) setting in bool_settings)
        {
            if (cityLookups.Contains(setting.lookup))
            {
                citySettingsBlock.Add(BattleSettings.createSettingCheckbox(setting));
            }
            else if (setting.lookup == "set-city-exists")
                settingsBlock.Add(BattleSettings.createSettingCheckbox(setting));
            else
                generalSettings.Add(BattleSettings.createSettingCheckbox(setting));
        }

        settingsBlock.Add(citySettingsBlock);
        settingsBlock.Add(generalSettings);

        mapGenerationPage.Add(settingsBlock);



        mapGenerationPage.Add(createButtonDisplay());
    }
    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");

        buttons.Add(BattleSettings.backButton());
        buttons.Add(MainMenu.Instance.backButton());
        return buttons;
    }
}
