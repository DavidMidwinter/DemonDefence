using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sergeant : BasePlayerUnit
{
    public int maxOrders;
    private int givenOrders;

    override protected void GameManagerStateChanged(GameState state)
    {
        if (state == GameState.PlayerTurn)
        {
            resetModifiers();
            givenOrders = 0;
        }
    }

    public override void onSelect()
    {
        if(givenOrders < maxOrders)
        {
            applyMovementBonus();
            givenOrders++;
        }
    }

    public void applyMovementBonus()
    {
        foreach(BasePlayerUnit playerUnit in UnitManager.Instance.allyUnits)
        {
            playerUnit.applyModifiers(move:1);
        }
    }
}
