using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public BasePlayerUnit target;
    public List<AStarNode> pathTiles;
    public float pathLength;
    public void Awake()
    {
        pathTiles = null;
    }

    public void selectAction()
    {

        if (UnitManager.Instance.allyUnits.Count > 0)
        {
            FindNearestTarget();
            if (checkRange(target))
            {
                StartCoroutine(makeAttack(target));
            }
            else
            {
                SetPath();
            }
        }
        else
        {
            Debug.Log("No target available.");
            takeAction();
            allowAction();
        }
    }
    public void FindNearestTarget()
    {
        Debug.Log("Find Target");
        target = UnitManager.Instance.allyUnits[0];
        foreach(BasePlayerUnit unit in UnitManager.Instance.allyUnits)
        {
            if((unit.transform.position - transform.position).magnitude < 
                (target.transform.position - transform.position).magnitude)
                target = unit;
        }
        

    }

    public void SetPath()
    {
        (List<AStarNode> aStarNodes, float pathLength) record =
            AStar.AStarPathfinder(OccupiedTile, target.OccupiedTile);
        Debug.Log(record);
        pathTiles = record.aStarNodes;
        pathLength = record.pathLength;
        List<AStarNode> movementPath;

        if (pathLength == 0)
        {
            takeAction();
            allowAction();
            return;
        }
        
        movementPath = new List<AStarNode>();
        foreach (AStarNode node in pathTiles)
        {
            if (node.g > maxMovement) break;
            else movementPath.Add(node);
        }
        movementPath[movementPath.Count - 1].referenceTile.SetUnit(this);
        movementPath.Reverse();
        path = processPath(movementPath);
        waypoint = path.Count - 1;
        takeAction();

    }

    public List<Vector3> processPath(List<AStarNode> nodes)
    {
        List<Vector3> waypoints = new List<Vector3>();
        foreach(AStarNode node in nodes)
        {
            waypoints.Add(node.referenceTile.transform.position);
        }
        return waypoints;
    }

    override public void allowAction()
    {
        if(getRemainingActions() > 0)
        {
            selectAction();
        }
        else
        {
            UnitManager.Instance.setNextEnemy();
        }

    }

    override public void takeAction()
    {
        remainingActions -= 1;
    }


}
