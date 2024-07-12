using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sergeant : BasePlayerLeader
{
    public int advance;
    public int defend;

    
    public override void onSelect()
    {
        Debug.Log($"Sergeant: {givenOrders} given out of {maxOrders} orders");
        if(givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            TacticalUI.Instance.addAction("Advance", applyMovementBonus);
            TacticalUI.Instance.addAction("Strike", applyStrengthBonus);
            TacticalUI.Instance.addAction("Defend", applyToughnessBonus);
        }
        else base.onSelect();
    }

    public void applyMovementBonus()
    {
        StartCoroutine(giveOrder(m: advance));
    }
    public void applyToughnessBonus()
    {
        StartCoroutine(giveOrder(t: defend));
    }
}
