using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
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


    private void FixedUpdate()
    {
        if(path != null)
        {
            FrameMove();
        }
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
            Debug.Log(nodeNeighbours.Count);
            List<NodeBase> possibleNodes = inRangeNodes.FindAll(n => nodeNeighbours.Contains(n.referenceTile));
            
            NodeBase nextNode = possibleNodes.Find(n => n.distance == current.distance - 1);
            current = nextNode;
        }
        waypoint = path.Count - 1;

        blockAction();

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


    public bool checkRange(BaseUnit target)
    {
        float range = (transform.position - target.transform.position).magnitude;
        return (range <= attackRange * 10);
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
        int result = Utils.rollDice();
        Debug.Log($"{this} attack against {target}: {result}");
        StartCoroutine(GameManager.Instance.PauseGame(3f));

        while (GameManager.Instance.isPaused)
        {
            yield return 0;
        }
        target.selectionMarker.SetActive(false);

        if (result > 5)
        {
            UnitManager.Instance.RemoveUnit(target);
            Destroy(target.gameObject);
        }
        TacticalUI.Instance.setCardText();
        if (UnitManager.Instance.checkRemainingUnits(faction))
        {
            takeAction();
            allowAction();
        }

    }
}
