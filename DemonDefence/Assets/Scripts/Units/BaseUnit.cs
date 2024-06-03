using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    private int unitHealth;
    private int maxHealth;
    public List<GameObject> individuals = new List<GameObject>();
    private List<GameObject> deadIndividuals = new List<GameObject>();
    public int individualHealth = 1;

    public GameObject selectionMarker;
    public Tile OccupiedTile;
    public Faction faction;
    public int maxMovement; 
    List<NodeBase> inRangeNodes;
    protected List<Vector3> path = null;
    public Rigidbody rb;
    public float movement_speed = 10;
    protected int waypoint = 0;
    public int maxActions;
    protected int remainingActions;
    public int attackRange;
    public int attackDamage = 1;
    public int strength;
    public int toughness;
    public UnitDisplay unitDisplay;
    
    public UnitAnimController animController;

    private void Start()
    {
        unitHealth = individualHealth * individuals.Count;
        maxHealth = unitHealth;
        setHealthBar();
    }
    private void FixedUpdate()
    {
        if(path != null)
        {
            unitDisplay.hideHealthBar();
            FrameMove();
            if (animController != null) animController.walk();
        }
        else
        {
            if (animController != null) animController.idle();
        }
        else unitDisplay.showHealthBar();
    }
    public bool isInRangeTile(Tile destination)
    {
        if (inRangeNodes.Count >= 0) return inRangeNodes.Exists(n => n.referenceTile == destination);
        else return false;
    }
    public bool isInRange(Vector3 location)
    {
        return (location - transform.position).magnitude <= maxMovement * 10;

    }

    public void calculateAllTilesInRange()
    {
        inRangeNodes = new List<NodeBase>();
        NodeBase originNode = new NodeBase(OccupiedTile, 0);
        inRangeNodes.Add(originNode);
        inRangeNodes = inRangeNodes[0].getValidTiles(maxMovement, faction);

    }

    public void createPath(Tile destination)
    {
        if (!inRangeNodes.Exists(n => n.referenceTile == destination)) return;

        path = new List<Vector3>();
        NodeBase destinationNode = inRangeNodes.Find(n => n.referenceTile == destination);
        NodeBase originNode = inRangeNodes.Find(n => n.referenceTile == OccupiedTile);
        NodeBase current = destinationNode;

        while (true)
        {

            if (current == originNode) break;
            path.Add(current.referenceTile.transform.position);
            //find nodes that are in inRangeNodes and are neighbours of previous node

            var nodeNeighbours = current.referenceTile.getNeighbours();
            List<NodeBase> possibleNodes = inRangeNodes.FindAll(n => nodeNeighbours.Contains(n.referenceTile));
            
            NodeBase nextNode = possibleNodes.Find(n => n.distance == current.distance - 1);
            current = nextNode;
        }
        waypoint = path.Count - 1;

        blockAction();

    }
    public bool amValidTarget(BaseUnit attacker)
    {

        return 
            (attacker.checkRange(this))
            && (attacker.faction != faction);
    }
    public void FrameMove()
    {
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
                allowAction();
                return;
            }
        }
        else
        {

            //calculate velocity for this frame
            Vector3 velocity = displacement;
            velocity.Normalize();
            velocity *= movement_speed;
            //apply velocity
            Vector3 newPosition = transform.position;
            newPosition += velocity * Time.deltaTime;
            rb.MovePosition(newPosition);

            //align to velocity
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity,
            10.0f * Time.deltaTime, 0f);
            Quaternion rotation = Quaternion.LookRotation(desiredForward);
            rb.MoveRotation(rotation);
        }
    }

    public void setRemainingActions(int actions)
    {
        remainingActions = actions;
    }

    public virtual void takeAction()
    {
        return;
    }

    public int getRemainingActions()
    {
        return remainingActions;
    }

    public virtual void allowAction()
    {
        return;
    }
    public virtual void blockAction()
    {
        return;
    }

    public float getDistance(BaseUnit target)
    {
        return (OccupiedTile.transform.position - target.OccupiedTile.transform.position).magnitude;
    }
    public bool checkRange(BaseUnit target)
    {
        return (getDistance(target) <= attackRange * 10);
    }

    public IEnumerator makeAttack(BaseUnit target)
    {
        blockAction();
        target.selectionMarker.SetActive(true);
        StartCoroutine(GameManager.Instance.PauseGame(1f));

        while (GameManager.Instance.isPaused)
        {
            yield return 0;
        }

        int threshold = Utils.calculateThreshold(strength, target.toughness);
        List<int> results = new List<int>();
        int dealtDamage = 0;
        foreach(GameObject soldier in individuals)
        {
            Debug.Log(soldier.name);
            Debug.Log(soldier.activeSelf);
            int attackRoll = Utils.rollDice();
            results.Add(attackRoll);
            if(attackRoll >= threshold)
            {
                dealtDamage += attackDamage;
            }
        }
        TacticalUI.Instance.DisplayResults(results.ToArray());
        StartCoroutine(GameManager.Instance.PauseGame(3f, false));

        while (GameManager.Instance.isPaused)
        {
            yield return 0;
        }
        target.selectionMarker.SetActive(false);

        target.takeDamage(dealtDamage);

        TacticalUI.Instance.ClearResults();
        if (UnitManager.Instance.checkRemainingUnits(faction))
        {
            takeAction();
            allowAction();
        }

    }

    public void takeDamage(int damage)
    {
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
        float scale = (float)unitHealth / (float)maxHealth;
        unitDisplay.setHealthBar(scale);
    }
}
