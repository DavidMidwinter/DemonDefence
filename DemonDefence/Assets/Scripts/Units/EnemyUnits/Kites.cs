using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Kites : BaseEnemyUnit
{
    bool canEvade;
    bool hasAttacked;
    public override void onSelect()
    {
        if (unitTypes.Contains(UnitType.Leader)){
            foreach(Kites member in detachmentMembers.Cast<Kites>())
            {
                member.allowEvasion();
            }
            allowEvasion();
            hasAttacked = false;
        }
        Debug.Log(canEvade);
        base.onSelect();
    }

    public override void checkCanAttack()
    {
        if (hasAttacked) canAttack = false;
        else base.checkCanAttack();
    }
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
                Debug.Log($"{this}[Kites]: Evading");
                SetPath();
                return;
            }
        }

        if (canAttack && findShootingTarget())
        {
            Debug.Log($"{this}[Kites]: Can attack a target");
            StartCoroutine(makeAttack(target));
            return;
        }

        FindNearestTarget();

        if (leader && getDistance(target) > 200)
        {
            if (getDistance(leader) < 30)
            {
                Debug.Log($"{this}[Kites]: Distance to leader less than 3 tiles");
                StartCoroutine(passTurn());
                return;
            }
            else 
            {
                Debug.Log($"{this}[Kites]: Distance to nearest enemy more than 20 tiles");
                if (pathLowOptimised(leader.OccupiedTile, 2))
                {
                    Debug.Log($"{this}[Kites]: Found path to leader");
                    setPathDjikstra(2);
                    return;
                }
                Debug.Log($"{this}[Kites]: Passing turn");
                StartCoroutine(passTurn());
                return;
            }
        }

        if (longDistancePath()) return;

        Debug.Log($"{this}[Kites]: Can take no actions");
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
            Debug.LogWarning($"{this}[Kites]: Target select error: {e}");
            target = null;
            return false;
        }

    }

    public IEnumerator makeAttack(BaseUnit target)
    {
        Debug.Log($"{this}[Kites]: Can evade: {canEvade}");
        hasAttacked = true;
        StartCoroutine(base.makeAttack(target, false));
        while (attacking)
        {
            yield return null;

        }
        if (canEvade && UnitManager.Instance.checkRemainingUnits(faction)) // If all units from the other team are dead, then gameplay is stopped by the unit manager; otherwise, gameplay can continue.
        {
            remainingActions = 1;
        }
        else
        {
            takeAction(attackActions);
        }
        Debug.Log($"{this}[Kites]: Calling allowAction");
        allowAction();
    }

    public bool evade()
    {
        Debug.Log($"{this}[Kites]: Evading");
        if (!canEvade) return false;
        FindNearestTarget();
        calculateAllTilesInRange(1);
        IEnumerable<DjikstraNode> moveNodes = inRangeNodes
            .Where(t => t.referenceTile.getDistance(target.OccupiedTile) >= 10 * (minimumRange + modifiers["minimumRange"]) && t.referenceTile.getDistance(OccupiedTile) >= 20);
        if (moveNodes.Count() == 0) return false;
        Tile movePoint = moveNodes
            .OrderBy(t => UnityEngine.Random.value)
            .First().referenceTile;

        return getPath(movePoint);
    }

    

    public override void resetStats()
    {
        target = null;
        canEvade = false;

        base.resetStats();
    }
    public override void addDetachmentMember(BaseUnit unit)
    {
        base.addDetachmentMember(unit);
        unit.setLeader(this);
    }
    public override void onDeath()
    {
        foreach (Kites member in detachmentMembers.Cast<Kites>())
        {
            member.setLeader();
        }
        base.onDeath();
    }

    public void allowEvasion()
    {
        canEvade = true;
    }


}
