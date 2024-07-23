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
            indirectFireOrder();
            strikeOrder();
            pointBlankFireOrder();
        }
        else base.onSelect();
    }
}
