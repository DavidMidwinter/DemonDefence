using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InfernalEngines : BaseEnemyUnit
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

        if (canAttack && findShootingTarget())
        {
            Debug.Log($"{this}[InfernalEngines]: Can attack a target");
            StartCoroutine(makeAttack(target));
            return;
        }

        FindNearestTarget();

        if (leader && getDistance(target) > 200)
        {
            if (getDistance(leader) < 30)
            {
                Debug.Log($"{this}[InfernalEngines]: Distance to leader less than 3 tiles");
                StartCoroutine(passTurn());
                return;
            }
            else
            {
                Debug.Log($"{this}[InfernalEngines]: Distance to nearest enemy more than 20 tiles");
                if (pathLowOptimised(leader.OccupiedTile, 2))
                {
                    Debug.Log($"{this}[InfernalEngines]: Found path to leader");
                    SetPath();
                    return;
                }
                Debug.Log($"{this}[InfernalEngines]: Passing turn");
                StartCoroutine(passTurn());
                return;
            }
        }

        if (longDistancePath()) return;

        Debug.Log($"{this}[InfernalEngines]: Can take no actions");
        StartCoroutine(passTurn());
        Debug.Log(pathTiles.Count);

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
            Debug.LogWarning($"{this}[InfernalEngines]: Target select error: {e}");
            target = null;
            return false;
        }

    }

    override public void resetModifiers()
    {
        base.resetModifiers();
        canAttackIndirect = true;
    }

    public override int getStrength(BaseUnit target)
    {
        int str = base.getStrength(target);

        if (!checkVisible(target))
            str -= 2;
        
        return str;
    }
}
