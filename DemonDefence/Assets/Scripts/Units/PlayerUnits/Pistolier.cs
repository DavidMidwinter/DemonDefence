using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistolier : BasePlayerLeader
{

    
    public override void onSelect()
    {
        Debug.Log($"Pistolier: {givenOrders} given out of {maxOrders} orders");
        if(givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            TacticalUI.Instance.addAction("Move\nand\nShoot", reduceAttackRequirement);
            TacticalUI.Instance.addAction("Strike", applyStrengthBonus);
            TacticalUI.Instance.addAction("Point-Blank\nFire", allowPointBlankFire);
        }
        else base.onSelect();
    }

    public void reduceAttackRequirement()
    {
        StartCoroutine(giveOrder(a: -1));
    }
    public void allowPointBlankFire()
    {
        StartCoroutine(giveOrder(mr: -3));
    }
}
