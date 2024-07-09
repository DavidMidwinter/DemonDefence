using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemyUnit : BaseUnit
{
    /// <summary>
    /// Contains functionality shared by all enemy units
    /// </summary>
    public BasePlayerUnit target;
    public List<AStarNode> pathTiles;
    public float pathLength;
    public BaseEnemyUnit leader;
    public void Awake()
    {
        pathTiles = null;
        selectionMarker.SetActive(false);
    }

    virtual public void selectAction()
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
                return;
            }
            else if(getDistance(target) < (minimumRange + modifiers["minimumRange"])){
                if (pathLowOptimised(target.OccupiedTile, (minimumRange + modifiers["minimumRange"]), 1))
                {
                    SetPath();
                    return;
                }
            }
            else
            {
                if (getPath(target))
                {
                    SetPath();
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
                foreach(BasePlayerUnit unit in getAccessibleTargets())
                {
                    target = unit;
                    if (getPath(target))
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
    public bool getPath(BaseUnit target)
    {
        /// Gets the shortest path to the target node using A* and return if it is valid
        /// Returns:
        /// bool: false if no valid path of length greater than 0 was found, true otherwise.
        return getPath(target.OccupiedTile);

    }
    public bool getPath(Tile target)
    {
        /// Gets the shortest path to the target node using A* and return if it is valid
        /// Returns:
        /// bool: false if no valid path of length greater than 0 was found, true otherwise.
        (List<AStarNode> aStarNodes, float pathLength) record =
            AStar.AStarPathfinder(OccupiedTile, target);
        if (record.pathLength == 0) return false;
        pathTiles = record.aStarNodes;
        pathLength = record.pathLength;
        return true;

    }
    public void SetPath()
    {
        /// Set which nodes will be in the path. This is determined by the amount of movement available.
        List<AStarNode> movementPath;

        int actionsToUse = totalMovementActionsRequired() > remainingActions ? remainingActions : totalMovementActionsRequired();

        movementPath = new List<AStarNode>();
        foreach (AStarNode node in pathTiles)
        {
            if (node.g > (maxMovement + modifiers["maxMovement"]) * actionsToUse) break;
            else movementPath.Add(node);
        }
        movementPath[movementPath.Count - 1].referenceTile.SetUnit(this);
        movementPath.Reverse();
        path = processPath(movementPath);
        waypoint = path.Count - 1;
        takeAction(actionsToUse - 1);
        fireAnimationEvent(animations.Walk);
        GameManager.Instance.updateTiles();

    }

    public bool pathLowOptimised(Tile destination, int distanceFromDestination = 0, int maxActionsToUse = 0)
    {
        /// Calculate a path using Djikstra's algorithm and A*. This is used when not in range of a player.
        /// Use of A* following target selection with Djikstra's is due to some issues where units do not move correctly.
        /// Args:
        ///     Tile destination: The tile to move towards
        ///     int distanceFromDestination: How far from the destination the path should terminate, e.g. if this is 2, a tile will only be selected if it is 2 or more tiles from destination. Default 0
        /// Returns:
        ///     bool: Whether a path could be made.
        ///     
        if (maxActionsToUse <= 0 || maxActionsToUse > remainingActions) maxActionsToUse = remainingActions;
        calculateAllTilesInRange(1 + ((maxActionsToUse - 1) * (maxMovement + modifiers["maxMovement"])));
        Debug.Log(name + ": " + remainingActions);
        foreach (DjikstraNode node in inRangeNodes)
            Debug.LogWarning(node.distance);
        DjikstraNode destinationNode = 
            inRangeNodes.
            Where(t => t.referenceTile.getDistance(destination) >= 10 * distanceFromDestination).
            OrderByDescending(t => t.distance).
            ThenBy(t => t.referenceTile.getDistance(destination)).
            ToList()[0];
        inRangeNodes.Clear();
        Debug.LogWarning(destinationNode.distance);
        return getPath(destinationNode.referenceTile);
    }

    protected int totalMovementActionsRequired()
    {
        int movement = maxMovement + modifiers["maxMovement"];
        int diff = (int)pathLength % (movement);
        int numberOfActions = (int)(pathLength - diff) / (movement);

        if (diff > 0) numberOfActions++;
        return numberOfActions;
    }

    public IEnumerator passTurn()
    {
        fireAnimationEvent(animations.Idle);
        StartCoroutine(GameManager.Instance.PauseGame(0.5f, false));
        while (GameManager.Instance.isPaused){
            yield return null;
        }
        takeAction(2);
        allowAction();
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
            base.allowAction();
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
            int roll = Utils.calculateThreshold(UnitManager.Instance.SelectedUnit.getStrength(this), getToughness());
            unitDisplay.setText($"{roll}+");
        }
        else
        {
            unitDisplay.setText(null);
        }
    }

    override public bool amValidTarget(BasePlayerUnit attacker)
    {
        /// Check if this unit is in range of an enemy unit's attack
        /// Args:
        /// BaseUnit attacker: The attacking unit
        /// Returns:
        /// bool: True if in range of the attacking unit and of a different faction; false otherwise
        /// 

        return (attacker.faction != faction) &&
            attacker.validTargets.Contains(this);
            
    }

    override protected void GameManagerStateChanged(GameState state)
    {
        if(state == GameState.EnemyTurn)
        {
            resetModifiers();
        }
    }

    public override void setLeader(BaseUnit unit = null)
    {
        leader = (BaseEnemyUnit)unit;
    }

}
