using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GreaterDemon : BaseEnemyUnit
{
    /// Functionality unique to the Greater Demon unit
    /// 

    public override void selectAction()
    {
        if (UnitManager.Instance.allyUnits.Count > 0)
        {
            FindNearestTarget();
            if (getDistance(target) < 300)
            {
                target = null;
                base.selectAction();
                return;
            }
            calculateAllTilesInRange();
            Debug.LogWarning(name + ": " + remainingActions);
            Tile destinationTile = inRangeNodes.OrderByDescending(t => t.distance).ThenBy(t => t.referenceTile.getDistance(target.OccupiedTile)).ToList()[0].referenceTile;
            inRangeNodes.Clear();
            if (getPath(destinationTile))
            {
                SetPath();
                return;
            }
        }

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
}
