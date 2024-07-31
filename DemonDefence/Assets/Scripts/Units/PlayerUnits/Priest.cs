using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : BasePlayerLeader
{
    private List<UnitType> bonusUnitTypes = new List<UnitType>
    { UnitType.Demonic };
    [SerializeField] private bool passiveAbilityGiven;

    public override void resetStats()
    {
        passiveAbilityGiven = false;
        base.resetStats();
    }

    public override void onSelect()
    {
        Debug.Log($"Priest: {givenOrders} given out of {maxOrders} orders");
        passiveStrength();
        if (givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            advanceOrder();
            defendOrder();
        }
        else base.onSelect();
    }
    public override void onDeath()
    {
        foreach (BasePlayerUnit playerUnit in aura)
        {
            playerUnit.setLeader();
        }
        base.onDeath();
    }

    public void passiveStrength()
    {
        if (passiveAbilityGiven) return;

        passiveAbilityGiven = true;

        giveStrengthAgainst(bonusUnitTypes.ToArray());
    }

    public override void addDetachmentMember(BaseUnit unit)
    {
        base.addDetachmentMember(unit);
        unit.setLeader(this);
    }

}
