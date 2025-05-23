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

    [HideInInspector]
    private int unitHealth;
    [HideInInspector]
    private int maxHealth;

    public GameObject characters;

    public List<GameObject> individuals = new List<GameObject>();

    [HideInInspector]
    private List<GameObject> deadIndividuals = new List<GameObject>();
    public int individualHealth = 1;

    public GameObject selectionMarker;
    public GameObject detachmentMarker;

    [HideInInspector]
    public Tile OccupiedTile;
    public Faction faction;
    public int maxMovement;

    [HideInInspector]
    public List<DjikstraNode> inRangeNodes;
    [HideInInspector]
    protected List<Vector3> path = null;

    public Rigidbody rb;
    public float movement_speed = 10;
    protected int waypoint = 0;
    public int maxActions;

    [HideInInspector]
    protected float playback_speed;

    [HideInInspector]
    protected int remainingActions;

    public int minimumRange, maximumRange;
    public bool defaultIndirectFire = false;
    public int attackDamage = 1;
    public int attackActions = 2;
    public int attackNumber = 1;
    public bool attackActionsRequired = false;
    public List<UnitType> strongAgainst;
    public List<UnitType> weakAgainst;
    public int strength;
    public int toughness;
    public UnitDisplay unitDisplay;

    [HideInInspector]
    public Dictionary<string, int> modifiers;

    [HideInInspector]
    protected List<BaseUnit> detachmentMembers = null;

    public event Action<animations> playAnimation;

    [SerializeField] public bool canAttackInWater = true;

    [HideInInspector]
    protected bool canAttack;

    [HideInInspector]
    protected bool canAttackIndirect = false;
    [HideInInspector]
    public bool attacking;

    [HideInInspector]
    protected BaseUnit attackTarget;

    int strengthPenalty => GameManager.Instance.strengthPenalty;
    [SerializeField] protected List<UnitType> currentStrongAgainst;
    [SerializeField] protected List<UnitType> currentWeakAgainst;

    [HideInInspector] protected Outline unitOutline;

    private void Start()
    {
        unitHealth = individualHealth * individuals.Count;
        maxHealth = unitHealth;
        modifiers = new Dictionary<string, int>();
        PlayerSettings.updateSetting += settingUpdate;
        playback_speed = PlayerSettings.getPlaybackSpeed();
        currentStrongAgainst = new List<UnitType>();
        currentWeakAgainst = new List<UnitType>();
        resetModifiers();
        setHealthBar();
        rb.detectCollisions = false;
        fireAnimationEvent(animations.Idle);
    }

    public void OnDestroy()
    {
        PlayerSettings.updateSetting -= settingUpdate;
    }

    protected void settingUpdate(string key)
    {
        if (key == "playback-speed")
        {
            playback_speed = PlayerSettings.getPlaybackSpeed();
        }
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
    public virtual void checkCanAttack()
    {
        bool validActions, validWater;
        if (attackActionsRequired)
            validActions = remainingActions < (attackActions + modifiers["attackActions"]) ? false : true;
        else validActions = true;

        if (!canAttackInWater)
            validWater = !OccupiedTile.isWater;
        else validWater = true;

        canAttack = validActions && validWater;
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

    public void calculateAllTilesInRange(int modifier = 0)
    {
        /// Gets all tiles that are in range of this unit's Tile
        /// Returns a list of NodeBase objects corrsponding to each tile that is in range
        /// Nodes are calculated in the NodeBase class
        inRangeNodes = new List<DjikstraNode>();
        DjikstraNode originNode = new DjikstraNode(OccupiedTile, 0);
        inRangeNodes.Add(originNode);
        inRangeNodes = inRangeNodes[0].getValidTiles(maxMovement+modifiers["maxMovement"]+modifier, faction);

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

            current = current.parent;
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

        if (dist <= 0.01 * (movement_speed * playback_speed))
        {
            transform.position = path[waypoint];
            waypoint--;
            if (waypoint < 0)
            {
                path = null;
                takeAction();
                allowAction();
                Debug.Log($"{this} has finished moving");
                fireAnimationEvent(animations.Idle);
                return;
            }
        }
        else
        {
            //calculate velocity for this frame
            Vector3 velocity = getVelocity(path[waypoint]);
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
        velocity *= movement_speed * playback_speed;
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
        /// 

        if(attackActionsRequired)
            canAttack = remainingActions < (attackActions+modifiers["attackActions"]) ? false : true;
        Debug.Log($"{this} allowing action");
        GameManager.Instance.updateTiles();
        TacticalUI.Instance.displayUnitCard(this);
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
        return OccupiedTile.checkClearLine(target.OccupiedTile);
    }
    public bool checkRange(BaseUnit target)
    {
        /// Check if a target is in range for this unit's Attack.
        /// Args:
        ///     BaseUnit target: an object of the BaseUnit type (or any child type)
        /// Returns:
        ///     Bool: true if target is in range, false otherwise
        return (
            canAttack &&
            getDistance(target) >= (minimumRange + modifiers["minimumRange"]) * 10 &&
            getDistance(target) <= (maximumRange + modifiers["maximumRange"]) * 10 &&
            (canAttackIndirect || checkVisible(target))
            );
    }
    public void setAttackTarget(BaseUnit target)
    {
        attackTarget = target;
    }
    virtual public IEnumerator makeAttack(BaseUnit target, bool handleAction = true)
    {
        /// Coroutine for making an attack. This requires making timed pauses, thus the use of a coroutine.
        /// Args:
        ///     BaseUnit target: The unit to attack
        ///   
        Debug.Log($"{this} is attacking {target}");
        attacking = true;
        hideHighlight();
        attackTarget = target;
        target.setAttackTarget(this);
        blockAction();
        target.selectionMarker.SetActive(true);
        while (attackTarget != null)
        {
            yield return 0;
        }
        StartCoroutine(GameManager.Instance.DelayGame(1f / playback_speed)); // The game is paused for 1 second before the attack is rolled (inversely proportional to animation speed).

        while (GameManager.Instance.delayingProcess)
        {
            yield return 0;
        }

        int threshold = Utils.calculateThreshold(getStrength(target), target.getToughness());
        List<(int result, bool pass)> results = new List<(int result, bool pass)>();
        int dealtDamage = 0;
        target.fireAnimationEvent(animations.Brace);
        fireAnimationEvent(animations.Attack);
        for (int i = 0; i < individuals.Count * attackNumber; i++) // Each individual in the squad makes one attack if they are alive.
        {
            int attackRoll = Utils.rollDice();
            results.Add((attackRoll, attackRoll >= threshold));
            if(attackRoll >= threshold)
            {
                dealtDamage += getAttackDamage();
            }
        }
        TacticalUI.Instance.DisplayResults(results.ToArray()); // This displays the results of each attack roll, with a 5 second pause so that the player has time to read them (inversely proportional to animation speed).
        StartCoroutine(GameManager.Instance.DelayGame(2.5f / playback_speed));

        while (GameManager.Instance.delayingProcess)
        {
            yield return 0;
        }

        target.takeDamage(dealtDamage); // Deal damage
        StartCoroutine(GameManager.Instance.DelayGame(2.5f / playback_speed));

        while (GameManager.Instance.delayingProcess)
        {
            yield return 0;
        }
        target.selectionMarker.SetActive(false);
        target.setAttackTarget(null);
        target.checkIfDead();
        attacking = false;
        attackTarget = null;
        TacticalUI.Instance.ClearResults();
        if (handleAction && UnitManager.Instance.checkRemainingUnits(faction)) // If all units from the other team are dead, then gameplay is stopped by the unit manager; otherwise, gameplay can continue.
        {
            takeAction(attackActions);
            Debug.Log($"{this} calling allowAction");
            allowAction();
        }
        if (remainingActions > 0) displayHighlight();

    }
    public void takeDamage(int damage)
    {
        /// Handles taking damage and removing individuals from the unit if enough damage has been taken. Also removes the unit from play if damage reaches 0.
        /// Args:
        ///     BaseUnit target: The unit to attack
        unitHealth -= damage;

        if (unitHealth < 0) unitHealth = 0;
        setHealthBar();
        while ((individuals.Count - 1) * individualHealth >= unitHealth && individuals.Count > 0)
            {
                GameObject character = individuals[0];
                individuals.Remove(character);
                deadIndividuals.Add(character);
                try
                {
                    character.GetComponentInChildren<AnimationController>().playDeathAnimation();
                }
                catch (Exception e){
                    Debug.LogWarning(e);
                }
                //character.SetActive(false);
            }
        if (unitHealth > 0)
        {
            fireAnimationEvent(animations.Idle);
        }
    }

    public void checkIfDead()
    {

        if (unitHealth <= 0)
        {
            Debug.Log("Unit killed");
            UnitManager.Instance.RemoveUnit(this);
            if (UnitManager.Instance.SelectedEnemy) UnitManager.Instance.SelectedEnemy.target = null;
            return;
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

    public virtual void resetModifiers()
    {
        Debug.Log($"Reset modifiers for {this}");
        modifiers["maxMovement"] = 0;
        modifiers["strength"] = 0;
        modifiers["toughness"] = 0;
        modifiers["attackDamage"] = 0;
        modifiers["attackActions"] = 0;
        modifiers["minimumRange"] = 0;
        modifiers["maximumRange"] = 0;
        
        currentStrongAgainst.Clear();
        currentStrongAgainst.AddRange(strongAgainst);

        currentWeakAgainst.Clear();
        currentWeakAgainst.AddRange(weakAgainst);
        canAttackIndirect = defaultIndirectFire;

    }

    public virtual void applyModifiers(
        int move = 0,
        int str = 0,
        int tough = 0,
        int dmg = 0,
        int attack = 0,
        int minrange = 0,
        int maxrange = 0,
        bool indirectFire = false
        )
    {
        Debug.Log($"Apply modifiers for {this}");
        modifiers["maxMovement"] += move;
        modifiers["strength"] += str;
        modifiers["toughness"] += tough;
        modifiers["attackDamage"] += dmg;
        modifiers["attackActions"] += attack;
        modifiers["minimumRange"] += minrange;
        modifiers["maximumRange"] += maxrange;
        canAttackIndirect = indirectFire;
    }

    public virtual void addStrongAgainst(UnitType[] unitTypes)
    {
        currentStrongAgainst.AddRange(unitTypes);
    }
    public virtual void addWeakAgainst(UnitType[] unitTypes)
    {
        currentWeakAgainst.AddRange(unitTypes);
    }

    public virtual void onSelect()
    {
        GameManager.Instance.updateTiles();
        TacticalUI.Instance.displayUnitCard(this);
    }

    virtual public int getStrength(BaseUnit target)
    {
        float totalStrength = strength;
        int comparison = 0;
        foreach(UnitType unitType in target.unitTypes)
        {
            if (currentStrongAgainst.Contains(unitType))
                comparison++;
            if (currentWeakAgainst.Contains(unitType))
                comparison--;
        }

        switch (comparison){
            case > 0:
                totalStrength *= 2;
                break;
            case < 0:
                totalStrength /= 2;
                break;
            default:
                break;
        }

        if ((getDistance(target) / 10 < minimumRange)
            || (attackActionsRequired && remainingActions < attackActions))
            totalStrength = totalStrength - strengthPenalty;

        totalStrength += modifiers["strength"];

        return (int)totalStrength;
    }
    public virtual int getMovement()
    {
        return maxMovement + modifiers["maxMovement"];
    }
    public virtual int getToughness()
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
        Debug.Log($"{this}: play animation {anim}");
        playAnimation?.Invoke(anim);
    }

    public virtual void resetStats()
    {
        /// Reset the stats for this unit for the beginning of the turn. Does not modify health.
        setRemainingActions(maxActions);
        resetModifiers();
        canAttack = true;
    }

    public void setUnitNameTag(string message)
    {
        unitDisplay.setName(message);
    }

    public void showName()
    {
        unitDisplay.showName();
    }
    public void hideName()
    {
        unitDisplay.hideName();
    }

    public string getName()
    {
        return unitDisplay.getName();
    }

    protected void initOutline(Color colour)
    {
        unitOutline = characters.AddComponent<Outline>();
        unitOutline.OutlineMode = Outline.Mode.OutlineAll;
        unitOutline.OutlineWidth = 5f;
        unitOutline.OutlineColor = colour;
        unitOutline.enabled = false;
    }
    public void displayHighlight()
    {
        unitOutline.enabled = true;
    }
    public void hideHighlight()
    {
        unitOutline.enabled = false;
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
