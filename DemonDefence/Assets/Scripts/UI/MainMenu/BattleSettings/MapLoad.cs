using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public static class MapLoad
{
    static VisualElement mapLoadPage;
    public static string saveDirectory => Utils.saveDirectory;
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

        DropdownMenu mapList = new DropdownMenu();

        mapLoadPage.Add(settingsBlock);
        mapLoadPage.Add(createButtonDisplay());

    }

    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");

        buttons.Add(BattleSettings.backButton());
        buttons.Add(MainMenu.Instance.backButton());
        return buttons;
    }
}
