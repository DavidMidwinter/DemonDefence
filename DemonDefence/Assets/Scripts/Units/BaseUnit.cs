using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    /// <summary>
    /// Contains functionality shared by all units
    /// </summary>
    /// 
    public List<UnitType> unitTypes;
    private int unitHealth;
    private int maxHealth;
    public List<GameObject> individuals = new List<GameObject>();
    private List<GameObject> deadIndividuals = new List<GameObject>();
    public int individualHealth = 1;

    public GameObject selectionMarker;
    public GameObject detachmentMarker;
    public Tile OccupiedTile;
    public Faction faction;
    public int maxMovement; 
    public List<DjikstraNode> inRangeNodes;
    protected List<Vector3> path = null;
    public Rigidbody rb;
    public float movement_speed = 10;
    protected int waypoint = 0;
    public int maxActions;
    protected int remainingActions;
    public int minimumRange, maximumRange;
    public int attackDamage = 1;
    public int attackActions = 2;
    public int strength;
    public int toughness;
    public UnitDisplay unitDisplay;

    public Dictionary<string, int> modifiers; 
    public List<UnitType> affectedTypes;

    protected List<BaseUnit> detachmentMembers = null;
    public event Action<animations> playAnimation;

    protected BaseUnit attackTarget;

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManagerStateChanged;
        unitHealth = individualHealth * individuals.Count;
        maxHealth = unitHealth;
        modifiers = new Dictionary<string, int>();
        setHealthBar();
        resetModifiers();
        rb.detectCollisions = false;
        fireAnimationEvent(animations.Idle);
    }
    private void FixedUpdate()
    {
        if(path != null)
        {
            unitDisplay.hideHealthBar();
            FrameMove();
        }
        else if(attackTarget != null)
        {
            unitDisplay.hideHealthBar();
            FrameRotate();
        }
        else unitDisplay.showHealthBar();
    }
    public bool isInRangeTile(Tile destination)
    {
        /// Calculate whether a tile is in movement range of this tile
        /// Args:
        /// Tile destination: The tile to check the range to
        /// 
        /// Returns:
        /// bool - Whether the tile is in range or not
        if (inRangeNodes.Count >= 0) return inRangeNodes.Exists(n => n.referenceTile == destination);
        else return false;
    }

    public void calculateAllTilesInRange()
    {
        /// Gets all tiles that are in range of this unit's Tile
        /// Returns a list of NodeBase objects corrsponding to each tile that is in range
        /// Nodes are calculated in the NodeBase class
        inRangeNodes = new List<DjikstraNode>();
        DjikstraNode originNode = new DjikstraNode(OccupiedTile, 0);
        inRangeNodes.Add(originNode);
        inRangeNodes = inRangeNodes[0].getValidTiles(maxMovement+modifiers["maxMovement"], faction);

    }

    public void createPath(Tile destination)
    {
        /// Creates a path using the inRangeNodes functionality
        /// Args:
        /// Tile destination: The tile to create a path to
        ///
        if (!inRangeNodes.Exists(n => n.referenceTile == destination)) return;

        path = new List<Vector3>();
        DjikstraNode destinationNode = inRangeNodes.Find(n => n.referenceTile == destination);
        DjikstraNode originNode = inRangeNodes.Find(n => n.referenceTile == OccupiedTile);
        DjikstraNode current = destinationNode;

        while (true)
        {

            if (current == originNode) break;
            path.Add(current.referenceTile.transform.position);
            //find nodes that are in inRangeNodes and are neighbours of previous node

            var nodeNeighbours = current.referenceTile.getNeighbours();
            List<DjikstraNode> possibleNodes = inRangeNodes.FindAll(n => nodeNeighbours.Contains(n.referenceTile));
            
            DjikstraNode nextNode = possibleNodes.Find(n => n.distance == current.distance - 1);
            current = nextNode;
        }
        fireAnimationEvent(animations.Walk);
        waypoint = path.Count - 1;

        blockAction();

    }
    
    public void FrameMove()
    {
        /// Calculate movement required for current frame.
        Vector3 displacement = path[waypoint] - transform.position;
        displacement.y = 0;
        float dist = displacement.magnitude;

        if (dist <= 0.01 * movement_speed)
        {
            transform.position = path[waypoint];
            waypoint--;
            if (waypoint < 0)
            {
                path = null;
                takeAction();
                allowAction();
                fireAnimationEvent(animations.Idle);
                return;
            }
        }
        else
        {
            //calculate velocity for this frame
            Vector3 velocity = getVelocity(path[waypoint]);
            Debug.Log(velocity);
            applyRotation(velocity);
            applyDisplacement(velocity);
        }
    }

    public void FrameRotate()
    {
        // Rotate towards the attack target
        Vector3 normalized = getNormalized(attackTarget.OccupiedTile.get3dLocation());

        if (checkRotate(normalized))
        {
            Vector3 velocity = normalized * movement_speed;
            Debug.Log(velocity);
            applyRotation(velocity);
        }
    }
    public Vector3 getVelocity(Vector3 target)
    {
        //calculate velocity for this frame
        Vector3 velocity = getNormalized(target);
        velocity *= movement_speed;
        return velocity;
    }

    public Vector3 getNormalized(Vector3 target)
    {
        /// Get the normalized vector to the target.
        /// Args:
        ///     Vector3 target: The point to get a vector to.
        /// Returns:
        ///     Vector3: A normalized vector
        Vector3 normalize = target - transform.position;
        normalize.Normalize();
        return normalize;
    }

    public bool checkRotate(Vector3 normalized)
    {
        /// Check if rotation is required
        /// Args:
        ///     Vector3 normalized: The direction that needs to be faced towards
        /// Returns:
        ///     bool: False if no rotation is required; true otherwise
        double threshold = (1 - (0.005 * movement_speed));

        if (Vector3.Dot(normalized, transform.forward) >= threshold)
        {
            attackTarget = null;
            transform.forward = normalized;
            return false;
        }
        return true;

    }
    public void applyRotation(Vector3 velocity)
    {
        //align to velocity
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity,
        10.0f * Time.deltaTime, 0f);
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        Debug.Log(rotation);
        rb.MoveRotation(rotation);
    }

    public void applyDisplacement(Vector3 velocity)
    {
        //apply velocity
        Vector3 newPosition = transform.position;
        newPosition += velocity * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    public void setRemainingActions(int actions)
    {
        /// Sets the Remaining Actions to the passed value.
        /// Args:
        ///     int actions: Number of actions to set Remaining Actions to.
        remainingActions = actions;
    }

    public virtual void takeAction(int actions = 1)
    {
        // Overridden by child classes.
        return;
    }

    public int getRemainingActions()
    {
        /// Get the remaining action points.
        /// Returns:
        ///     int: Number of actions remaining.
        return remainingActions;
    }

    public virtual void allowAction()
    {
        /// Perform any functionality required for allowing a new Action. Overridden in child classes
        GameManager.Instance.updateTiles();
        return;
    }
    public virtual void blockAction()
    {
        /// Perform any functionality required for blocking subsequent Actions. Overridden in child classes
        GameManager.Instance.clearTiles();
        return;
    }

    public float getDistance(BaseUnit target)
    {
        /// Get the distance between this unit and a target unit.
        /// Args:
        ///     BaseUnit target: an object of the BaseUnit type (or any child type)
        /// Returns:
        ///     The distance as a float
        return (OccupiedTile.getDistance(target.OccupiedTile));
    }

    public bool checkVisible(BaseUnit target)
    {
        return (OccupiedTile.checkClearLine(target.OccupiedTile));
    }
    public bool checkRange(BaseUnit target)
    {
        /// Check if a target is in range for this unit's Attack.
        /// Args:
        ///     BaseUnit target: an object of the BaseUnit type (or any child type)
        /// Returns:
        ///     Bool: true if target is in range, false otherwise
        return (
            getDistance(target) >= minimumRange * 10 &&
            getDistance(target) <= maximumRange * 10 &&
            checkVisible(target)
            );
    }
    public IEnumerator makeAttack(BaseUnit target)
    {
        /// Coroutine for making an attack. This requires making timed pauses, thus the use of a coroutine.
        /// Args:
        ///     BaseUnit target: The unit to attack
        ///     
        attackTarget = target;
        blockAction();
        target.selectionMarker.SetActive(true);
        Vector3 normalized = getNormalized(attackTarget.OccupiedTile.get3dLocation());
        while (attackTarget != null)
        {
            yield return 0;
        }
        fireAnimationEvent(animations.Idle);
        StartCoroutine(GameManager.Instance.PauseGame(1f, false)); // The game is paused for 1 second before the attack is rolled.

        while (GameManager.Instance.isPaused)
        {
            yield return 0;
        }

        int threshold = Utils.calculateThreshold(getStrength(), target.getToughness());
        List<int> results = new List<int>();
        int dealtDamage = 0;
        fireAnimationEvent(animations.Attack);
        foreach(GameObject soldier in individuals) // Each individual in the squad makes one attack if they are alive.
        {
            Debug.Log(soldier.name);
            Debug.Log(soldier.activeSelf);
            int attackRoll = Utils.rollDice();
            results.Add(attackRoll);
            if(attackRoll >= threshold)
            {
                dealtDamage += getAttackDamage();
            }
        }
        TacticalUI.Instance.DisplayResults(results.ToArray(), faction); // This displays the results of each attack roll, with a 3 second pause so that the player has time to read them.
        StartCoroutine(GameManager.Instance.PauseGame(3f, false));

        while (GameManager.Instance.isPaused)
        {
            yield return 0;
        }
        target.selectionMarker.SetActive(false);

        target.takeDamage(dealtDamage); // Deal damage

        TacticalUI.Instance.ClearResults();
        if (UnitManager.Instance.checkRemainingUnits(faction)) // If all units from the other team are dead, then gameplay is stopped by the unit manager; otherwise, gameplay can continue.
        {
            takeAction(attackActions);
            allowAction();
        }

    }

    public void takeDamage(int damage)
    {
        /// Handles taking damage and removing individuals from the unit if enough damage has been taken. Also removes the unit from play if damage reaches 0.
        /// Args:
        ///     BaseUnit target: The unit to attack
        unitHealth -= damage;
        setHealthBar();

        if(unitHealth <= 0)
        {
            Debug.Log("Unit killed");
            UnitManager.Instance.RemoveUnit(this);
            if (UnitManager.Instance.SelectedEnemy) UnitManager.Instance.SelectedEnemy.target = null;
            return;
        }
        else
        {
            while((individuals.Count - 1) * individualHealth >= unitHealth)
            {
                GameObject character = individuals[0];
                individuals.Remove(character);
                deadIndividuals.Add(character);
                character.SetActive(false);
            }
        }
    }

    public void setHealthBar()
    {
        /// Set the health bar for this unit.
        unitDisplay.setHealthDisplay(unitHealth, maxHealth);
    }

    public virtual bool amValidTarget(BasePlayerUnit attacker)
    {
        return false;
    }

    public void resetModifiers()
    {
        modifiers["maxMovement"] = 0;
        modifiers["strength"] = 0;
        modifiers["toughness"] = 0;
        modifiers["attackDamage"] = 0;
    }

    protected virtual void GameManagerStateChanged(GameState state)
    {

    }

    public void applyModifiers(int move = 0, int str = 0, int tough = 0, int dmg = 0)
    {
        modifiers["maxMovement"] += move;
        modifiers["strength"] += str;
        modifiers["toughness"] += tough;
        modifiers["attackDamage"] += dmg;
    }

    public virtual void onSelect()
    {
        GameManager.Instance.updateTiles();
    }

    public int getStrength()
    {
        return strength + modifiers["strength"];
    }
    public int getMovement()
    {
        return maxMovement + modifiers["maxMovement"];
    }
    public int getToughness()
    {
        return toughness + modifiers["toughness"];
    }

    public int getAttackDamage()
    {
        return attackDamage + modifiers["attackDamage"];
    }

    public virtual void addDetachmentMember(BaseUnit unit)
    {
        if (detachmentMembers == null) detachmentMembers = new List<BaseUnit>();
        detachmentMembers.Add(unit);
    }
    public virtual void removeDetachmentMember(BaseUnit unit)
    {
        detachmentMembers.Remove(unit);
    }

    public virtual void onDeath()
    {
        return;
    }

    public virtual void setLeader(BaseUnit unit = null)
    {
        return;
    }

    public bool unitIsInDetachment(BaseUnit unit)
    {
        if (detachmentMembers == null) return false;
        return detachmentMembers.Contains(unit);
    }

    public void setDetachmentColour(Material colour)
    {
        detachmentMarker.GetComponent<MeshRenderer>().material = colour;
    }

    public void fireAnimationEvent(animations anim)
    {
        playAnimation?.Invoke(anim);
    }

    public virtual void resetStats()
    {
        /// Reset the stats for this unit for the beginning of the turn. Does not modify health.
        setRemainingActions(maxActions);
        resetModifiers();
    }
}


public enum UnitType
{
    Common,
    Pious,
    Mechanised,
    Cultist,
    Demonic,
    Despoiler,
    Leader
}
