using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction faction;
    public int maxMovement; 
    List<NodeBase> inRangeNodes;

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

    
}
