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
                SetPath();
                return;
            }
            StartCoroutine(passTurn());
        }

        if (canAttack && findShootingTarget())
        {
            StartCoroutine(makeAttack(target));
            return;
        }
        
        FindNearestTarget();

        if (leader)
        {
            if (getDistance(leader) < 30)
                StartCoroutine(passTurn());
            else if (getDistance(target) > 200)
            {
                if (pathLowOptimised(leader.OccupiedTile, 2))
                {
                    SetPath();
                    return;
                }
                StartCoroutine(passTurn());
            }
        }

        int actions;
        if (remainingActions == 1) actions = 1;
        else if (getDistance(target) > 20 * (maxMovement + modifiers["maxMovement"])) actions = 0;
        else actions = 1;

        if (pathLowOptimised(target.OccupiedTile, 
            1 + (minimumRange+modifiers["minimumRange"]), actions))
        {
            SetPath();
            return;
        }

        else
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
            Debug.LogWarning($"target select error: {e}");
            target = null;
            return false;
        }

    }

    public IEnumerator makeAttack(BaseUnit target)
    {
        StartCoroutine(base.makeAttack(target, false));
        while (attacking)
        {
            yield return null;
            Debug.LogWarning("waiting");

        }
        takeAction();
        if (UnitManager.Instance.checkRemainingUnits(faction)) // If all units from the other team are dead, then gameplay is stopped by the unit manager; otherwise, gameplay can continue.
        {
            remainingActions = 1;
            canAttack = false;
        }
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