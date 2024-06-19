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
        base.onSelect();
        if(givenOrders < maxOrders)
        {
            TacticalUI.Instance.addAction("Advance", applyMovementBonus);
            TacticalUI.Instance.addAction("Strike", applyStrengthBonus);
            TacticalUI.Instance.addAction("Defend", applyToughnessBonus);
        }
    }

    public void giveOrder(int m = 0, int s = 0, int t = 0)
    {
        blockAction();
        foreach (BasePlayerUnit playerUnit in UnitManager.Instance.allyUnits)
        {
            playerUnit.applyModifiers(move: m, str: s, tough: t);
        }
        givenOrders++;
        if (givenOrders >= maxOrders)
        {
            TacticalUI.Instance.clearActions();
        }
        GameManager.Instance.PauseGame(1f);
        takeAction();
        allowAction();
    }

    public void applyMovementBonus()
    {
        giveOrder(m: 1);
    }
    public void applyStrengthBonus()
    {
        giveOrder(s: 1);
    }
    public void applyToughnessBonus()
    {
        giveOrder(t: 1);
    }
}
