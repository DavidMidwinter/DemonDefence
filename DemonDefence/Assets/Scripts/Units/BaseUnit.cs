using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction faction;
    public int maxMovement; 
    List<NodeBase> inRangeNodes;
    List<Vector3> path;

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
            path.Add(current.referenceTile.locationVector);
            //find nodes that are in inRangeNodes and are neighbours of previous node

            var nodeNeighbours = current.referenceTile.getNeighbours();
            Debug.Log(nodeNeighbours.Count);
            List<NodeBase> possibleNodes = inRangeNodes.FindAll(n => nodeNeighbours.Contains(n.referenceTile));
            
            NodeBase nextNode = possibleNodes.Find(n => n.distance == current.distance - 1);
            current = nextNode;
        }

    }
}
