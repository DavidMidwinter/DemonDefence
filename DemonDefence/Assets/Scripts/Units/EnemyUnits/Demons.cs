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
            if (getDistance(target) < 100)
            {
                target = null;
                base.selectAction();
                return;
            }
        }

        if(getDistance(leader) < 30)
        {
            takeAction(2);
            allowAction();
            return;
        }

        if (getPath(leader))
        {
            pathTiles.RemoveAt(pathTiles.Count - 1);
            pathLength -= 1;

            if (pathTiles.Count > 0)
            {
                SetPath();
                return;
            }
        }
        takeAction();
        allowAction();

        
    }
}
