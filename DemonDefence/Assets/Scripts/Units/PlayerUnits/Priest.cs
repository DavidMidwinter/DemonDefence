using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : BasePlayerLeader
{
    private List<UnitType> bonusUnitTypes = new List<UnitType>
    { UnitType.Demonic };

    public override void onSelect()
    {
        Debug.Log($"Pistolier: {givenOrders} given out of {maxOrders} orders");
        if (givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            advanceOrder();
            giveStrengthAgainstOrder();
            defendOrder();
        }
        else base.onSelect();
    }

    public IEnumerator giveKeywordOrder(List<UnitType> unitTypes)
    {
        blockAction();
        fireAnimationEvent(animations.Order);
        foreach (BasePlayerUnit playerUnit in aura)
        {
            playerUnit.addStrongAgainst(bonusUnitTypes.ToArray());
        }
        givenOrders++;
        if (givenOrders >= maxOrders)
        {
            TacticalUI.Instance.clearActions();
        }
        StartCoroutine(GameManager.Instance.PauseGame(3f, false));
        while (GameManager.Instance.isPaused) yield return null;
        takeAction(0);
        allowAction();
    }

    public void giveStrengthAgainst()
    {
        StartCoroutine(giveKeywordOrder(bonusUnitTypes));
    }

    public void giveStrengthAgainstOrder()
    {
        addAbilityButton("Bless\nagainst\nDemons", giveStrengthAgainst);
    }
}
