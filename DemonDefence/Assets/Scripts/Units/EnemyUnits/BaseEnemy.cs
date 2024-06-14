using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    /// <summary>
    /// Contains functionality shared by all enemy units
    /// </summary>
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
        /// Selects an action to take. If there is a target selected, continue to move/attack that target; otherwise, find the nearest
        /// enemy unit then attack. This avoids having to recalculate the target every action, preventing lag between actions.
        /// If the nearest enemy is in attack range, attack it. If not, move towards the nearest enemy unit that can be reached.
        /// If no enemy unit can be reached, pass the action.
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
        // Finds the nearest target using absolute distance. Is quicker than checking accessible targets but is less useful for movement.
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
        /// Gets all possible targets. Currently, this is any unit with at least one neighbour tile that can be walked on
        /// This is to remove units that can be determined as unreachable without needing to calculate any paths. 
        /// It also orders this list by distance.
        /// Returns:
        ///     List<BasePlayerUnit>: A list of all BasePlayerUnit objects that are theoretically reachable, in order of distance to this unit.
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
        /// Gets the shortest path to the target node using A* and return if it is valid
        /// Returns:
        /// bool: false if no valid path of length greater than 0 was found, true otherwise.
        (List<AStarNode> aStarNodes, float pathLength) record =
            AStar.AStarPathfinder(OccupiedTile, target.OccupiedTile);
        if (record.pathLength == 0) return false;
        pathTiles = record.aStarNodes;
        pathLength = record.pathLength;
        return true;

    }
    public void SetPath()
    {
        /// Set which nodes will be in the path. This is determined by the amount of movement available.
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
        /// Converts a list of A* nodes into a list of Vector£'s that can be used for moving a unit.
        /// Args:
        ///     List<AStarNode> nodes: The A* node list to convert
        /// Returns:
        ///     List<Vector3> waypoints: A list of all the Vector3 positions scraped from the nodes.
        List<Vector3> waypoints = new List<Vector3>();
        foreach(AStarNode node in nodes)
        {
            waypoints.Add(node.referenceTile.transform.position);
        }
        return waypoints;
    }

    override public void allowAction()
    {
        /// Functionality to allow a new action to be taken. If there are no action points remaining, clear target and path data in preparation for next turn.
        if (getRemainingActions() > 0)
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
        /// Functionality to take action points.
        /// Args:
        ///     int actions: Number of action points to be taken
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
