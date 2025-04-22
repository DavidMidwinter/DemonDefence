using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sergeant : BasePlayerLeader
{
    public override void onSelect()
    {
        Debug.Log($"{this}[Sergeant]: {givenOrders} given out of {maxOrders} orders");
        if(givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            advanceOrder();
            strikeOrder();
            defendOrder();
        }
        else base.onSelect();
    }
}
