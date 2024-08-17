using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public static class UnitStats
{
    private static VisualElement statPage;

    public static VisualElement getStatPage()
    {
        if(statPage is null)
        {
            createDetachmentPage();
        }

        return statPage;
    }

    private static void createDetachmentPage()
    {
        statPage = UITools.Create("page", "white-border", "detachments");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Detachments";

        statPage.Add(header);

        VisualElement detachmentBlock = UITools.Create("detachment-page");

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

        Label playerHeader = UITools.Create<Label>("header-text");
        playerHeader.text = "Player";
        playerUnits.Add(playerHeader);

        Label enemyHeader = UITools.Create<Label>("header-text");
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

        statPage.Add(detachmentBlock);
        statPage.Add(createButtonDisplay());
    }

    static VisualElement createDetachmentCard(ScriptableDetachment detachment)
    {
        VisualElement card = UITools.Create("white-border", "detachment-card");

        Label detachmentName = UITools.Create<Label>("detachment-header-text");
        detachmentName.text = detachment.unitName;
        card.Add(detachmentName);
        Label detachmentSize = UITools.Create<Label>("detachment-troop-number-text");
        detachmentSize.text = $"Number of Troops: {detachment.numberOfTroops}";
        card.Add(detachmentSize);

        VisualElement leaderCard = createUnitCard(detachment.leaderUnit, detachment.leaderImage);
        VisualElement unitCard = createUnitCard(detachment.troopUnit, detachment.troopImage);

        card.Add(leaderCard);
        Label leaderAbility = UITools.Create<Label>("instruction-text", "unit-info-text");
        leaderAbility.text = string.Join("\n", detachment.leaderAbilities);
        card.Add(leaderAbility);

        card.Add(unitCard);

        return card;
    }

    static VisualElement createUnitCard(BaseUnit unit, Texture2D unitimg)
    {
        VisualElement card = UITools.Create("unit-card");
        VisualElement mainInfo = UITools.Create("unit-main-body");
        VisualElement imageDisplay = UITools.Create("unit-info");
        Image img = UITools.Create<Image>("unit-image");
        img.image = unitimg;
        imageDisplay.Add(img);

        VisualElement statsDisplay1 = UITools.Create("unit-info");
        Label unitStats = UITools.Create<Label>("instruction-text", "unit-info-text");
        unitStats.text =
            $"Movement: {unit.maxMovement}\n" +
            $"Members: {unit.individuals.Count}\n" +
            $"Health: {unit.individualHealth} / {unit.individualHealth * unit.individuals.Count}\n" +
            $"Toughness: {unit.toughness}";
        Label name = UITools.Create<Label>("unit-name");
        name.text = unit.name;

        statsDisplay1.Add(name);
        statsDisplay1.Add(unitStats);

        VisualElement statsDisplay2 = UITools.Create("unit-info");
        Label weaponStats = UITools.Create<Label>("instruction-text", "unit-info-text");
        weaponStats.text =
            $"Range: {unit.minimumRange} - {unit.maximumRange}\n" +
            $"Strength: {unit.strength}\n" +
            $"Damage: {unit.attackDamage}\n" +
            $"Actions: {unit.maxActions}";

        if (unit.attackActionsRequired)
        {
            weaponStats.text += $" [required to attack]";
        }

        Label blank = UITools.Create<Label>("unit-name");
        statsDisplay2.Add(blank);
        statsDisplay2.Add(weaponStats);

        mainInfo.Add(imageDisplay);
        mainInfo.Add(statsDisplay1);
        mainInfo.Add(statsDisplay2);

        card.Add(mainInfo);


        Label types = UITools.Create<Label>("instruction-text", "unit-info-text");
        types.text = $"Unit Types: {string.Join(", ", unit.unitTypes)}\n" +
            $"Strong Against: {string.Join(", ", unit.strongAgainst)}\n" +
            $"Weak Against: {string.Join(", ", unit.weakAgainst)}";
        card.Add(types);

        return card;
    }

    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");
        buttons.Add(MainMenu.Instance.backButton());
        return buttons;
    }
}
