using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            Debug.Log($"{this}[Demons]: {this} passing turn");
            StartCoroutine(passTurn());
            return;
        }

        else 
        {
            
            if (pathLowOptimised(leader.OccupiedTile))
            {
                setPathDjikstra();
                return;
            }
        }
        Debug.Log($"{this}[Demons]: {this} passing turn");
        StartCoroutine(passTurn());


    }
}
