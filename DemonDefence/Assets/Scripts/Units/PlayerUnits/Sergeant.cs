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

    
    public override void onSelect()
    {
        Debug.Log("Sergeant");
        Debug.Log($"{givenOrders} given out of {maxOrders} orders");
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

    public IEnumerator giveOrder(int m = 0, int s = 0, int t = 0)
    {
        blockAction();
        fireAnimationEvent(animations.Order);
        foreach (BasePlayerUnit playerUnit in aura)
        {
            playerUnit.applyModifiers(move: m, str: s, tough: t);
        }
        givenOrders++;
        if (givenOrders >= maxOrders)
        {
            TacticalUI.Instance.clearActions();
        }
        StartCoroutine(GameManager.Instance.PauseGame(3f, false));
        while (GameManager.Instance.isPaused) yield return null;
        takeAction(0);
        allowAction();
    }

    public void applyMovementBonus()
    {
        StartCoroutine(giveOrder(m: advance));
    }
    public void applyStrengthBonus()
    {
        StartCoroutine(giveOrder(s: strike));
    }
    public void applyToughnessBonus()
    {
        StartCoroutine(giveOrder(t: defend));
    }

    public override void resetStats()
    {
        /// Reset given orders along with other stats
        givenOrders = 0;
        base.resetStats();
    }
}
