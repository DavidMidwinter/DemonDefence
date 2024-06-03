using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public bool attacking;
    public BasePlayerUnit target;
    public List<AStarNode> pathTiles;
    public float pathLength;
    public void Awake()
    {
        pathTiles = null;
        selectionMarker.SetActive(false);
    }

    public void selectAction()
    {
        if(target != null)
        {
            if (checkRange(target))
            {
                StartCoroutine(makeAttack(target));
                attacking = true;
                return;
            }
            else
            {
                if (getPath())
                {
                    SetPath();
                    attacking = false;
                    return;
                }
            }
        }

        if (UnitManager.Instance.allyUnits.Count > 0)
        {
            FindNearestTarget();
            if (checkRange(target))
            {
                StartCoroutine(makeAttack(target));
                attacking = true;
                return;
            }
            else
            {
                foreach(BasePlayerUnit unit in getAccessibleTargets())
                {
                    target = unit;
                    if (getPath())
                    {
                        SetPath();
                        attacking = false;
                        return;
                    }
                }
                Debug.Log("No target accessable.");
                takeAction();
                allowAction();
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
            if(getDistance(unit) < getDistance(target))
                target = unit;
        }
        

    }

    public List<BasePlayerUnit> getAccessibleTargets()
    {
        Dictionary<BasePlayerUnit, float> accessible_targets = new Dictionary<BasePlayerUnit, float>();
        foreach(BasePlayerUnit unit in UnitManager.Instance.allyUnits)
        {
            if(unit.OccupiedTile.getNeighbours().Exists(t => t.Walkable))
            {
                accessible_targets.Add(unit, getDistance(unit));
            }
        }
        return accessible_targets.OrderBy(unit => unit.Value).
            ToDictionary(unit => unit.Key, unit => unit.Value).
            Keys.ToList();
    }
    public bool getPath()
    {
        (List<AStarNode> aStarNodes, float pathLength) record =
            AStar.AStarPathfinder(OccupiedTile, target.OccupiedTile);
        if (record.pathLength == 0) return false;
        pathTiles = record.aStarNodes;
        pathLength = record.pathLength;
        return true;

    }
    public void SetPath()
    {
        List<AStarNode> movementPath;

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
            attacking = false;
            selectAction();
        }
        else
        {
            target = null;
            pathTiles = null;
            pathLength = 0;
            path = null;
            UnitManager.Instance.setNextEnemy();
        }

    }

    override public void takeAction(int actions = 1)
    {
        remainingActions -= actions;
    }

    public void Update()
    {
        if (UnitManager.Instance.SelectedUnit && amValidTarget(UnitManager.Instance.SelectedUnit))
        {
            int roll = Utils.calculateThreshold(UnitManager.Instance.SelectedUnit.strength, toughness);
            unitDisplay.setText($"{roll}+");
        }
        else
        {
            unitDisplay.setText(null);
        }
    }
}
