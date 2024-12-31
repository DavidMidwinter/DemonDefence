using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MapLoad
{
    static VisualElement mapLoadPage;
    public static VisualElement getMapLoadPage(bool forceGenerate = false)
    {
        if (forceGenerate || mapLoadPage is null)
        {
            createMapLoadPage();
        }

        return mapLoadPage;
    }

    static void createMapLoadPage()
    {

        mapLoadPage = UITools.Create("page", "white-border");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Load Map";

        mapLoadPage.Add(header);

        ScrollView settingsBlock = new ScrollView(ScrollViewMode.Vertical);
        settingsBlock.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        settingsBlock.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        settingsBlock.AddToClassList("unity-scroll-view__content-container");
        settingsBlock.AddToClassList("settings-block");

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
