using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction faction;
    public int maxMovement;
    public List<Tile> inRangeTiles;

    public bool isInRangeTile(Tile destination)
    {
        if (inRangeTiles.Count >= 0) return inRangeTiles.Contains(destination);
        else return false;
    }
    public bool isInRange(Vector3 location)
    {
        return (location - transform.position).magnitude <= maxMovement * 10;

    }

    public void calculateAllTilesInRange()
    {
        List<NodeBase> nodes = new List<NodeBase>();
        NodeBase originNode = new NodeBase(OccupiedTile, 0);
        nodes.Add(originNode);
        nodes = nodes[0].getValidTiles(maxMovement, nodes);

        inRangeTiles = new List<Tile>();

        foreach (NodeBase n in nodes){
            inRangeTiles.Add(n.referenceTile);
        }

    }

    
}
