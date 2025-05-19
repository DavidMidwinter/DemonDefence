using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemyUnit : BaseUnit
{
    /// <summary>
    /// Contains functionality shared by all enemy units
    /// </summary>
    /// 


    [HideInInspector] public BasePlayerUnit target;
    [HideInInspector] public List<AStarNode> pathTiles;
    [HideInInspector] public List<DjikstraNode> djikstraPathTiles;
    [HideInInspector] public float pathLength;
    [HideInInspector] public BaseEnemyUnit leader;
    public void Awake()
    {
        pathTiles = null;
        selectionMarker.SetActive(false);
        initOutline(Color.red);
        UnitManager.Instance.resetEnemies += resetStats;
    }

    new public void OnDestroy()
    {

        UnitManager.Instance.resetEnemies -= resetStats;
        base.OnDestroy();
    }

    public override void onSelect()
    {
        pathTiles = null;
        djikstraPathTiles = null;
        pathLength = 0;
        path = null;
        base.onSelect();
    }

    virtual public void selectAction()
    {
        /// Selects an action to take. If there is a target selected, continue to move/attack that target; otherwise, find the nearest
        /// enemy unit then attack. This avoids having to recalculate the target every action, preventing lag between actions.
        /// If the nearest enemy is in attack range, attack it. If not, move towards the nearest enemy unit that can be reached.
        /// If no enemy unit can be reached, pass the action.
        /// 
        int kiteRange = minimumRange + modifiers["minimumRange"];
        if (target != null)
        {
            if (checkRange(target))
            {
                StartCoroutine(makeAttack(target));
                return;
            }
            else if(getDistance(target) < (minimumRange + modifiers["minimumRange"])){
                if (pathLowOptimised(target.OccupiedTile, (minimumRange + modifiers["minimumRange"]), 1, true))
                {
                    setPathDjikstra();
                    return;
                }
            }
            else
            {
                if (pathLowOptimised(target.OccupiedTile, (minimumRange + modifiers["minimumRange"]), 2, true))
                {
                    setPathDjikstra(2);
                    return;
                }
            }
        }

        if (UnitManager.Instance.allyUnits.Count > 0)
        {
            FindNearestTarget();
            if (checkRange(target) && canAttack)
            {
                StartCoroutine(makeAttack(target));
                attacking = true;
                return;
            }
            else if (getDistance(target) < (minimumRange + modifiers["minimumRange"]))
            {
                if (getPath(target))
                {
                    SetPath();
                    attacking = false;
                    return;
                }
            }
            else
            {
                if (pathLowOptimised(target.OccupiedTile, (minimumRange + modifiers["minimumRange"]), 2, true))
                {
                    setPathDjikstra();
                    return;
                }
                Debug.Log($"{this}[BaseEnemyUnit]: No target accessable.");
                takeAction();
                allowAction();
            }
        }
        else
        {
            Debug.Log($"{this}[BaseEnemyUnit]: No target available.");
            takeAction();
            allowAction();
        }
    }
    public void FindNearestTarget()
    {
        // Finds the nearest target using absolute distance. Is quicker than checking accessible targets but is less useful for movement.
        Debug.Log($"{this}[BaseEnemyUnit]: Find Target");
        target = UnitManager.Instance.allyUnits[0];
        foreach(BasePlayerUnit unit in UnitManager.Instance.allyUnits)
        {
            if(getDistance(unit) < getDistance(target))
                target = unit;
        }
        

    }

    public GateTile GetGateByWeight(int index = 0)
    {
        
        List<GateTile> tiles = GridManager.Instance.getGates().OrderBy(t => t.getDistance(OccupiedTile) - t.getDistance(target.OccupiedTile)).ToList();
        while (index < 0) index = tiles.Count + index;


        if (tiles.Count > 0) return tiles[index];
        else return null;

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
    public void SetPath(float offset = 0, int maxActionsToUse = 0)
    {
        /// Set which nodes will be in the path. This is determined by the amount of movement available.
        List<AStarNode> movementPath;
        int actionLimit = maxActionsToUse > 0 ? maxActionsToUse : totalMovementActionsRequired();

        int actionsToUse = actionLimit > remainingActions ? remainingActions : actionLimit;

        movementPath = new List<AStarNode>();
        foreach (AStarNode node in pathTiles)
        {
            if (node.g > (maxMovement + modifiers["maxMovement"]) * actionsToUse + offset) break;
            else movementPath.Add(node);
        }

        movementPath[movementPath.Count - 1].referenceTile.SetUnit(this);
        movementPath.Reverse();
        path = processPath(movementPath);
        waypoint = path.Count - 1;
        takeAction(actionsToUse - 1);
        fireAnimationEvent(animations.Walk);
        GameManager.Instance.updateTiles();
        pathTiles.RemoveRange(0, movementPath.Count - 1);

    }

    public bool pathLowOptimised(Tile destination, int distanceFromDestination = 0, int maxActionsToUse = 0, bool closestToDestination = false)
    {
        /// Calculate a path using Djikstra's algorithm and A*. This is used when not in range of a player.
        /// Use of A* following target selection with Djikstra's is due to some issues where units do not move correctly.
        /// Args:
        ///     Tile destination: The tile to move towards
        ///     int distanceFromDestination: How far from the destination the path should terminate, e.g. if this is 2, a tile will only be selected if it is 2 or more tiles from destination. Default 0
        /// Returns:
        ///     bool: Whether a path could be made.
        ///     
        Debug.Log($"{this}[BaseEnemyUnit]: distance from destination: {distanceFromDestination}");
        if (maxActionsToUse <= 0) maxActionsToUse = remainingActions;
        calculateAllTilesInRange(1 + ((maxActionsToUse - 1) * (maxMovement + modifiers["maxMovement"])));
        DjikstraNode destinationNode = nodeSelector(destination, distanceFromDestination, closestToDestination);
        Debug.Log($"{this}[BaseEnemyUnit]: destinationNode exists: {destinationNode is not null}");
        if (destinationNode is null) return false;
        createPathDjikstra(destinationNode);
        inRangeNodes.Clear();
        Debug.Log($"{this}[BaseEnemyUnit]: path nodes: {djikstraPathTiles.Count}");
        if (djikstraPathTiles.Count > 0) return true;
        else return false;
    }

    public void createPathDjikstra(DjikstraNode destinationNode)
    {
        /// Creates a path using the inRangeNodes functionality
        /// Args:
        /// Tile destination: The tile to create a path to
        ///

        djikstraPathTiles = new List<DjikstraNode>();
        DjikstraNode originNode = inRangeNodes.Find(n => n.referenceTile == OccupiedTile);
        DjikstraNode current = destinationNode;

        while (true)
        {

            if (current == originNode) break;
            djikstraPathTiles.Add(current);
            //find nodes that are in inRangeNodes and are neighbours of previous node

            current = current.parent;
        }
        djikstraPathTiles.Reverse();


    }

    public void setPathDjikstra(int actionsToUse = 1, int offsetActions = 0)
    {
        /// Sets a path mapped using Djikstra's algorithm
        /// Args:
        ///     int actionsToUse: Number of actions to use while moving, default 1

        Debug.Log($"{this}[BaseEnemyUnit]: djikstraPathTiles: {djikstraPathTiles.Count};");
        List<DjikstraNode> nodes = new List<DjikstraNode>();
        int distance = (actionsToUse+offsetActions) * (maxMovement + modifiers["maxMovement"]);
        foreach(DjikstraNode node in djikstraPathTiles)
        {
            // If node is greater than the max distance to move, break
            if (node.distance > distance) break;

            nodes.Add(node);

            // If node is within attack range, change target and break
            if (UnitManager.Instance.allyUnits.Exists(u => Utils.calculateDistance(u.OccupiedTile.get2dLocation(), node.referenceTile.get2dLocation()) <= (maximumRange + modifiers["maximumRange"]))) {
                target = UnitManager.Instance.allyUnits.Find(u => Utils.calculateDistance(u.OccupiedTile.get2dLocation(), node.referenceTile.get2dLocation()) <= (maximumRange + modifiers["maximumRange"]));
                break;
            }
        }
        djikstraPathTiles.RemoveRange(0, nodes.Count);
        nodes.Reverse();
        Debug.Log($"{this}[BaseEnemyUnit]: Number of Djikstra nodes: {nodes.Count}");
        path = new List<Vector3>();
        foreach (DjikstraNode node in nodes)
        {
            path.Add(node.referenceTile.get3dLocation());
        }
        nodes[0].referenceTile.SetUnit(this);
        fireAnimationEvent(animations.Walk);
        waypoint = path.Count - 1;
        takeAction(actionsToUse - 1);
        blockAction();
    }
    virtual public DjikstraNode nodeSelector(Tile destination, int distanceFromDestination, bool closestToDestination = false)
    {
        IEnumerable<DjikstraNode> possibleList = inRangeNodes.
            Where(t => t.referenceTile.getDistance(destination) >= 10 * distanceFromDestination);
        if (closestToDestination)
            possibleList = possibleList.OrderBy(t => t.referenceTile.getDistance(destination));
        else
            possibleList = possibleList.OrderByDescending(t => t.distance).
            ThenBy(t => t.referenceTile.getDistance(destination));

        (float distance, float distanceToDestination)[] selectedNodes = possibleList.Select(x => (x.distance, x.referenceTile.getDistance(destination))).ToArray();
        Debug.Log($"{possibleList.Count()}, {selectedNodes.Count()}");
        Debug.Log($"Nodes: [{string.Join(", ", selectedNodes)}]");
        if (possibleList.Count() > 0)
            return possibleList.First();
        else return null;
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
        StartCoroutine(GameManager.Instance.DelayGame(0.5f));
        while (GameManager.Instance.delayingProcess){
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
            checkCanAttack();
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
            Debug.Log($"{this} is passing to next enemy");
            UnitManager.Instance.setNextEnemy();
            fireAnimationEvent(animations.Idle);
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

    public override void setLeader(BaseUnit unit = null)
    {
        leader = (BaseEnemyUnit)unit;
    }

    public bool longDistancePath()
    {
        int actions = 3;

        if (djikstraPathTiles != null && djikstraPathTiles.Count > 0)
        {
            Debug.LogWarning($"{this}[BaseEnemyUnit]: Path already calculated");
            setPathDjikstra(offsetActions: maxActions - remainingActions);
            return true;
        }
        else if (pathLowOptimised(target.OccupiedTile,
            1 + (minimumRange + modifiers["minimumRange"]), actions))
        {
            setPathDjikstra();
            Debug.LogWarning($"{this} found path to a target");
            Debug.LogWarning($"{this}[BaseEnemyUnit]: {remainingActions} actions remaining");
            return true;
        }

        return false;
    }

}
