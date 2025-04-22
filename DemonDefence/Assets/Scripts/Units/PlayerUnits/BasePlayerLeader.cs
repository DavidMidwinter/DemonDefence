using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerLeader : BasePlayerUnit
{

    [HideInInspector] protected int givenOrders;
    public int maxOrders;
    [HideInInspector] public int strike = 2;
    [HideInInspector] public int advance = 2;
    [HideInInspector] public int defend = 2;
    public IEnumerator giveOrder(int m = 0, int s = 0, int t = 0, int a = 0, int mr = 0, bool indf = false, bool giveToSelf = true)
    {
        blockAction();
        fireAnimationEvent(animations.Order);
        foreach (BasePlayerUnit playerUnit in aura)
        {
            if (playerUnit == this && !giveToSelf) continue;
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
        StartCoroutine(GameManager.Instance.DelayGame(3f / playback_speed));
        while (GameManager.Instance.delayingProcess) yield return null;
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
    public override void resetStats()
    {
        /// Reset given orders along with other stats
        givenOrders = 0;
        base.resetStats();
    }
    // Abilities
    public void applyMovementBonus()
    {
        StartCoroutine(giveOrder(m: advance));
    }
    public void applyToughnessBonus()
    {
        StartCoroutine(giveOrder(t: defend));
    }
    public void applyStrengthBonus()
    {
        StartCoroutine(giveOrder(s: strike));
    }
    public void allowindirectFire()
    {
        StartCoroutine(giveOrder(
            m: - maxMovement,
            indf: true, 
            s: -2,
            giveToSelf: false));
    }
    public void allowPointBlankFire()
    {
        StartCoroutine(giveOrder(mr: -3));
    }
    public void reduceAttackRequirement()
    {
        StartCoroutine(giveOrder(a: -1));
    }


    public void giveStrengthAgainst(UnitType[] unitTypes, bool giveToSelf = false)
    {
        Debug.Log($"{this}[BasePlayerLeader]: Give strength for {this} detachment");
        foreach (BasePlayerUnit playerUnit in detachmentMembers)
        {
            if (playerUnit == this && !giveToSelf) continue;
            playerUnit.addStrongAgainst(unitTypes);
        }
    }


    // Orders

    public void advanceOrder()
    {
        addAbilityButton("Advance", applyMovementBonus);
    }
    public void strikeOrder()
    {
        addAbilityButton("Strike", applyStrengthBonus);
    }
    public void defendOrder()
    {
        addAbilityButton("Defend", applyToughnessBonus);
    }
    public void pointBlankFireOrder()
    {
        addAbilityButton("Point-blank\nFire", allowPointBlankFire);
    }
    public void moveAndShootOrder()
    {
        addAbilityButton("Move\nand\nShoot", reduceAttackRequirement);
    }
    public void indirectFireOrder()
    {
        addAbilityButton("Indirect Fire", allowindirectFire);
    }
}
