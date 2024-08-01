using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultLeader : BaseEnemyUnit
{
    /// Functionality unique to the Cult Leader unit
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
}
