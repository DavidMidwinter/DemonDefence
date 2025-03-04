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

    private static VisualElement buttonDisplay => statPage.Q<VisualElement>(className: "buttons");

    public static VisualElement getStatPage(Button backButton, bool forceGenerate = false)
    {
        if(forceGenerate || statPage is null)
        {
            createDetachmentPage();
        }
        buttonDisplay.Clear();
        buttonDisplay.Add(backButton);
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
        foreach (ScriptableDetachment detachment in DetachmentData.getAllDetachments())
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

        VisualElement leaderCard = createUnitCard(detachment.leaderUnit);
        VisualElement unitCard = createUnitCard(detachment.troopUnit, detachment.numberOfTroops);

        card.Add(leaderCard);

        card.Add(unitCard);

        return card;
    }

    static VisualElement createUnitCard(ScriptableUnit unit, int amount = 0)
    {
        VisualElement card = UITools.Create("unit-card", "white-border-thin");
        VisualElement mainInfo = UITools.Create("unit-main-body");
        VisualElement imageDisplay = UITools.Create("unit-info");
        Image img = UITools.Create<Image>("unit-image");
        img.image = unit.unitImage;
        imageDisplay.Add(img);

        VisualElement statsDisplay1 = UITools.Create("unit-info");
        Label unitStats = UITools.Create<Label>("instruction-text", "unit-info-text");
        unitStats.text =
            $"Movement: {unit.unitPrefab.maxMovement}\n" +
            $"Members: {unit.unitPrefab.individuals.Count}\n" +
            $"Health: {unit.unitPrefab.individualHealth} / {unit.unitPrefab.individualHealth * unit.unitPrefab.individuals.Count}\n" +
            $"Toughness: {unit.unitPrefab.toughness}\n" +
            $"Max actions: {unit.unitPrefab.maxActions}";
        Label name = UITools.Create<Label>("unit-name");

        name.text = unit.unitName;

        if (amount > 0) name.text = name.text + $" ({amount})";

        statsDisplay1.Add(name);
        statsDisplay1.Add(unitStats);

        VisualElement statsDisplay2 = UITools.Create("unit-info");
        Label weaponStats = UITools.Create<Label>("instruction-text", "unit-info-text");
        weaponStats.text =
            $"Attacks: {unit.unitPrefab.attackNumber} / {unit.unitPrefab.attackNumber * unit.unitPrefab.individuals.Count}\n" +
            $"Range: {unit.unitPrefab.minimumRange} - {unit.unitPrefab.maximumRange}\n" +
            $"Strength: {unit.unitPrefab.strength}\n" +
            $"Damage: {unit.unitPrefab.attackDamage}\n";

        if (unit.unitPrefab.attackActions >= unit.unitPrefab.maxActions)
        {
            weaponStats.text += $"Attacks end the turn";
        }

        Label blank = UITools.Create<Label>("unit-name");
        statsDisplay2.Add(blank);
        statsDisplay2.Add(weaponStats);

        mainInfo.Add(imageDisplay);
        mainInfo.Add(statsDisplay1);
        mainInfo.Add(statsDisplay2);

        card.Add(mainInfo);


        Label types = UITools.Create<Label>("instruction-text", "unit-info-text", "white-border-thin");
        types.text = $"Unit Types: {string.Join(", ", unit.unitPrefab.unitTypes)}";
        Label strongAgainst = UITools.Create<Label>("instruction-text", "unit-info-text", "white-border-thin");
        strongAgainst.text =$"Strong Against: {string.Join(", ", unit.unitPrefab.strongAgainst)}";
        Label weakAgainst = UITools.Create<Label>("instruction-text", "unit-info-text", "white-border-thin");
        weakAgainst.text =$"Weak Against: {string.Join(", ", unit.unitPrefab.weakAgainst)}";
        Label restrictions = UITools.Create<Label>("instruction-text", "unit-info-text", "white-border-thin");
        restrictions.text = $"Restrictions: {checkRestrictions(unit.unitPrefab)}";

        card.Add(types);
        card.Add(strongAgainst);
        card.Add(weakAgainst);
        card.Add(restrictions);

        if(unit.abilities.Length > 0)
        {
            Label abilities = UITools.Create<Label>("instruction-text", "unit-info-text", "white-border-thin");
            abilities.text ="Abilities:\n" + string.Join("\n", unit.abilities);
            card.Add(abilities);
        }

        return card;
    }
    private static string checkRestrictions(BaseUnit unit)
    {
        List<string> restrictions = new List<string>();
        if (!unit.canAttackInWater) restrictions.Add("Cannot attack while in water");
        if (unit.attackActionsRequired) restrictions.Add($"Must have {unit.attackActions} actions available to attack");

        
        return restrictions.Count > 0 ? string.Join("\n", restrictions) : "None";
    }
    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");
        return buttons;
    }
}
