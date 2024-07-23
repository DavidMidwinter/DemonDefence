using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtillerySergeant : BasePlayerLeader
{

    
    public override void onSelect()
    {
        Debug.Log($"Artillery Sergeant: {givenOrders} given out of {maxOrders} orders");
        if(givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            TacticalUI.Instance.addAction("Indirect Fire", indirectFire);
            TacticalUI.Instance.addAction("Strike", applyStrengthBonus);
            TacticalUI.Instance.addAction("Point-Blank\nFire", allowPointBlankFire);
        }
        else base.onSelect();
    }

    public void indirectFire()
    {
        StartCoroutine(giveOrder(m: -3, indf: true, giveToSelf: false));
    }
    public void allowPointBlankFire()
    {
        StartCoroutine(giveOrder(mr: -3));
    }
}
