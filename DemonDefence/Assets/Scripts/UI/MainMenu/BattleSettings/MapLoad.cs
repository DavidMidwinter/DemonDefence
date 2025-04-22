using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public static class MapLoad
{
    static VisualElement mapLoadPage;
    public static string saveDirectory => Utils.saveDirectory;

    public static DropdownField mapMenu => mapLoadPage.Q<DropdownField>(className: "set-map-name");
    public static DropdownField spawnMapList => mapLoadPage.Q<DropdownField>(className: "set-spawn-map");
    public static VisualElement getMapLoadPage(bool forceGenerate = false)
    {
        if (forceGenerate || mapLoadPage is null)
        {
            createMapLoadPage();
        }
        Directory.CreateDirectory(saveDirectory);

        return mapLoadPage;
    }

    static void createMapLoadPage()
    {

        mapLoadPage = UITools.Create("page", "white-border");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Load Map";

        mapLoadPage.Add(header);

        ScrollView settingsBlock = UITools.Create(ScrollViewMode.Vertical, "settings-block");

        settingsBlock.Add(createSettingDropdown("Select Map", "", "set-map-name", "dropdown-display"));
        settingsBlock.Add(createSpawnmapDropdown());

        mapLoadPage.Add(settingsBlock);
        mapLoadPage.Add(createButtonDisplay());

        Directory.CreateDirectory(saveDirectory);
        foreach (string map in Directory.GetDirectories(saveDirectory))
        {
            mapMenu.choices.Add(Path.GetFileName(map));
        }

    }

    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");

        buttons.Add(BattleSettings.backButton());
        buttons.Add(MainMenu.Instance.backButton());
        return buttons;
    }

    private static void setValue(string lookup, string value)
    {
        BattleSettings.setValue(lookup, value);
        Debug.LogWarning($"[MapLoad]: Lookup: {lookup}, Value: {value}");
        switch (lookup)
        {
            case "set-map-name":
                PopulateSpawnmapDropdown(value);
                break;
            default:
                break;
        }
    }

    private static DropdownField createSettingDropdown(string name, string initial, string lookup, string displayclass = "setting-display")
    {

        DropdownField dropDown = UITools.CreateDropdown(name, initial, lookup, displayclass, "instruction-text");

        dropDown.RegisterValueChangedCallback(evt => setValue(lookup, evt.newValue));
        setValue(lookup, initial);

        return dropDown;
    }

    public static void resetMenuSelection()
    {
        mapMenu.value = mapMenu.choices[0];
    }

    private static DropdownField createSpawnmapDropdown()
    {
        DropdownField spawnMapList = createSettingDropdown("Spawnmaps:", null, "set-spawn-map", "dropdown-display");
        return spawnMapList;
    }


    private static void PopulateSpawnmapDropdown(string selectedMap)
    {
        if (spawnMapList is null) return;
        spawnMapList.choices.Clear();
        if (selectedMap is not null)
        {
            foreach (string spawnMap in Directory.GetFiles(Path.Combine(saveDirectory, selectedMap, "spawnmaps")))
            {
                if (Path.GetFileName(spawnMap).Contains(".meta")) continue;
                spawnMapList.choices.Add(Path.GetFileName(spawnMap));
            }
        }
        spawnMapList.SetValueWithoutNotify(null);
        setValue("set-spawn-map", null);
    }

}
