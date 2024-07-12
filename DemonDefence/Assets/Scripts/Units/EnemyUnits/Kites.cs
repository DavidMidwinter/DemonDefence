using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Kites : BaseEnemyUnit
{

    override public void selectAction()
    {
        /// Selects an action to take. If there is a target selected, continue to move/attack that target; otherwise, find the nearest
        /// enemy unit then attack. This avoids having to recalculate the target every action, preventing lag between actions.
        /// If the nearest enemy is in attack range, attack it. If not, move towards the nearest enemy unit that can be reached.
        /// If no enemy unit can be reached, pass the action.
        /// 
        if (!UnitManager.Instance.checkRemainingUnits(faction))
            return;

        if (!canAttack)
        {
            if (evade())
            {
                Debug.Log($"{this} evading");
                SetPath();
                return;
            }
        }

        if (canAttack && findShootingTarget())
        {
            Debug.Log($"{this} can attack a target");
            StartCoroutine(makeAttack(target));
            return;
        }

        FindNearestTarget();

        if (leader)
        {
            if (getDistance(leader) < 30)
            {
                Debug.Log($"{this} distance to leader less than 3 tiles");
                StartCoroutine(passTurn());
                return;
            }
            else if (getDistance(target) > 200)
            {
                Debug.Log($"{this} distance to nearest enemy more than 20 tiles");
                if (pathLowOptimised(leader.OccupiedTile, 2))
                {
                    Debug.Log($"{this} found path to leader");
                    SetPath();
                    return;
                }
                Debug.LogWarning($"{this} passing turn");
                StartCoroutine(passTurn());
                return;
            }
        }

        int actions;
        if (remainingActions == 1) actions = 1;
        else if (getDistance(target) > 20 * (maxMovement + modifiers["maxMovement"])) actions = 0;
        else actions = 1;

        if (pathLowOptimised(target.OccupiedTile,
            1 + (minimumRange + modifiers["minimumRange"]), actions))
        {
            Debug.Log($"{this} found path to a target");
            SetPath();
            return;
        }

        Debug.LogWarning($"{this} can take no actions");
        StartCoroutine(passTurn());

    }

    public bool findShootingTarget()
    {
        try
        {
            target = UnitManager.Instance.allyUnits.Where(t => checkRange(t)).OrderBy(t => getDistance(t)).First();
            return (target != null);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{this} target select error: {e}");
            target = null;
            return false;
        }

    }

    public IEnumerator makeAttack(BaseUnit target)
    {
        canAttack = false;
        StartCoroutine(base.makeAttack(target, false));
        while (attacking)
        {
            yield return null;

        }
        if (UnitManager.Instance.checkRemainingUnits(faction)) // If all units from the other team are dead, then gameplay is stopped by the unit manager; otherwise, gameplay can continue.
        {
            remainingActions = 1;
        }
        else
        {
            takeAction(attackActions);
        }
        Debug.Log($"{this} calling allowAction");
        allowAction();
    }

    public bool evade()
    {
        FindNearestTarget();
        calculateAllTilesInRange(1);
        Tile movePoint = inRangeNodes
            .Where(t => t.referenceTile.getDistance(target.OccupiedTile) >= 10 * (minimumRange + modifiers["minimumRange"]) && t.referenceTile.getDistance(OccupiedTile) >= 20)
            .OrderBy(t => UnityEngine.Random.value)
            .First().referenceTile;

        return getPath(movePoint);
    }

    public override void resetStats()
    {
        target = null;
        base.resetStats();
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
}
