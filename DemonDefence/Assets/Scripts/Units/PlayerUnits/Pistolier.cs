using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistolier : BasePlayerLeader
{

    
    public override void onSelect()
    {
        Debug.Log($"{this}[Pistolier]: {givenOrders} given out of {maxOrders} orders");
        if(givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            base.onSelect();
            moveAndShootOrder();
            strikeOrder();
            pointBlankFireOrder();
        }
        else base.onSelect();
    }
}
