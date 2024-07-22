using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerLeader : BasePlayerUnit
{

    [HideInInspector] protected int givenOrders;
    public int maxOrders;
    public int strike;
    public IEnumerator giveOrder(int m = 0, int s = 0, int t = 0, int a = 0, int mr = 0, bool indf = false)
    {
        blockAction();
        fireAnimationEvent(animations.Order);
        foreach (BasePlayerUnit playerUnit in aura)
        {
            playerUnit.applyModifiers(
                move: m, 
                str: s, 
                tough: t, 
                attack: a,
                minrange: mr,
                indirectFire: indf
                );
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
    public void applyStrengthBonus()
    {
        StartCoroutine(giveOrder(s: strike));
    }
    public override void resetStats()
    {
        /// Reset given orders along with other stats
        givenOrders = 0;
        base.resetStats();
    }
}
