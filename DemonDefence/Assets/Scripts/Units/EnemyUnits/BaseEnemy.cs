using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public BasePlayerUnit target;
    public List<Tile> pathTiles;
    public void Awake()
    {
        pathTiles = null;
    }
    public void FindNearestTarget()
    {
        Debug.Log("Find Target");
        target = UnitManager.Instance.allyUnits[0];
        foreach(BasePlayerUnit unit in UnitManager.Instance.allyUnits)
        {
            if((unit.transform.position - transform.position).magnitude < (target.transform.position - transform.position).magnitude)
            {
                target = unit;
            }
        }

        pathTiles = AStar.AStarPathfinder(OccupiedTile, target.OccupiedTile);

    }
}
