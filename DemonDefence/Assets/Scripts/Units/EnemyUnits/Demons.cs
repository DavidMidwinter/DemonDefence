using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demons : BaseEnemyUnit
{
    /// Functionality unique to the Demon unit
    /// 
    public override void selectAction()
    {
        if (leader == null) { 
            base.selectAction();
            return;
        }

        if (UnitManager.Instance.allyUnits.Count > 0)
        {
            FindNearestTarget();
            if (getDistance(target) < 10)
            {
                target = null;
                base.selectAction();
                return;
            }
        }

        if (getPath(leader))
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
