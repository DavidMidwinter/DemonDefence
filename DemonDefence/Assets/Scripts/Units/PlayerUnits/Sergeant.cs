using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sergeant : BasePlayerUnit
{
    public int maxOrders;
    public int advance;
    public int strike;
    public int defend;
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
            getAffected(maxMovement);
        }
    }

    public override void allowAction()
    {
        aura.Clear();
        if (givenOrders < maxOrders)
        {
            getAffected(maxMovement);
            TacticalUI.Instance.enableOrders();
        }
        base.allowAction();
    }

    public override void blockAction()
    {
        TacticalUI.Instance.disableOrders();
        base.blockAction();
    }

    public void giveOrder(int m = 0, int s = 0, int t = 0)
    {
        blockAction();
        foreach (BasePlayerUnit playerUnit in aura)
        {
            playerUnit.applyModifiers(move: m, str: s, tough: t);
        }
        givenOrders++;
        if (givenOrders >= maxOrders)
        {
            TacticalUI.Instance.clearActions();
        }
        GameManager.Instance.PauseGame(1f);
        takeAction(0);
        allowAction();
    }

    public void applyMovementBonus()
    {
        giveOrder(m: advance);
    }
    public void applyStrengthBonus()
    {
        giveOrder(s: strike);
    }
    public void applyToughnessBonus()
    {
        giveOrder(t: defend);
    }
}
