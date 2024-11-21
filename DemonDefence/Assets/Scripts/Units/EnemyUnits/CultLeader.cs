using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultLeader : BaseEnemyUnit
{
    /// Functionality unique to the Cult Leader unit
    /// 
    [SerializeField] int moveModifier, strengthModifier, toughnessModifier;
    [HideInInspector] private bool hasGivenBlessing;

    public override void resetModifiers()
    {
        hasGivenBlessing = false;
        base.resetModifiers();
    }
    public override void selectAction()
    {
        if (!hasGivenBlessing)
        {
            Debug.Log($"{this} giving out blessing");
            StartCoroutine(giveBlessing());
            hasGivenBlessing = true;
            return;
        }
        if (UnitManager.Instance.allyUnits.Count > 0)
        {
            FindNearestTarget();
            if (getDistance(target) < 300)
            {
                target = null;
                base.selectAction();
                return;
            }
            if (pathLowOptimised(target.OccupiedTile))
            {
                SetPath();
                return;
            }
        }
        Debug.LogWarning($"{this} passing turn");

        StartCoroutine(passTurn());

    }
    public override void addDetachmentMember(BaseUnit unit)
    {
        base.addDetachmentMember(unit);
        unit.setLeader(this);
    }

    public override void onDeath()
    {
        foreach (BaseUnit unit in detachmentMembers) unit.setLeader();
        base.onDeath();
    }

    public IEnumerator giveBlessing()
    {
        blockAction();
        fireAnimationEvent(animations.Order);
        foreach (BaseEnemyUnit unit in detachmentMembers)
        {
            unit.applyModifiers(
                move: moveModifier,
                str: strengthModifier,
                tough: toughnessModifier
                );
        }
        StartCoroutine(GameManager.Instance.DelayGame(3f));
        while (GameManager.Instance.delayingProcess) yield return null;
        takeAction(0);
        allowAction();
    }
}
