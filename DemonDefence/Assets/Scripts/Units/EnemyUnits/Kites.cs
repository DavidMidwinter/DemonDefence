using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Kites : BaseEnemyUnit
{
    override public void selectAction()
    {
        /// Selects an action to take. If there is a target selected, continue to move/attack that target; otherwise, find the nearest
        /// enemy unit then attack. This avoids having to recalculate the target every action, preventing lag between actions.
        /// If the nearest enemy is in attack range, attack it. If not, move towards the nearest enemy unit that can be reached.
        /// If no enemy unit can be reached, pass the action.
        /// 
        if(target is null)
            FindNearestTarget();
        if (target != null)
        {
            if (checkRange(target))
            {
                StartCoroutine(makeAttack(target));
                canAttack = false;
                return;
            }
            else if (getDistance(target) < (minimumRange + modifiers["minimumRange"]))
            {
                if (pathLowOptimised(target.OccupiedTile, (minimumRange + modifiers["minimumRange"]), 1))
                {
                    SetPath();
                    return;
                }
            }
            else
            {
                if (pathLowOptimised(target.OccupiedTile, (minimumRange + modifiers["minimumRange"]), 1))
                {
                    SetPath();
                    return;
                }
            }
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
        remainingActions = 1;
        if (UnitManager.Instance.checkRemainingUnits(faction)) // If all units from the other team are dead, then gameplay is stopped by the unit manager; otherwise, gameplay can continue.
        {
            calculateAllTilesInRange(1);
            Tile movePoint = inRangeNodes
                .Where(t => t.referenceTile.getDistance(target.OccupiedTile) >= 10 * (minimumRange + modifiers["minimumRange"]) && t.referenceTile.getDistance(OccupiedTile) >= 20)
                .OrderBy(t => Random.value)
                .First().referenceTile;

            if (getPath(movePoint))
            {
                SetPath();
            }
            else
            {
                takeAction();
                allowAction();
            }
        }
    }

    public override DjikstraNode nodeSelector(Tile destination, int distanceFromDestination)
    {
        return inRangeNodes.
            Where(t => t.referenceTile.getDistance(destination) >= 10 * distanceFromDestination).
            OrderByDescending(t => t.distance).
            ToList()[0];
    }
}
