using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBase
{
    public Tile referenceTile;
    public bool visited;
    public int distance;
    public NodeBase(Tile input, int defsaultDistance = 1000)
    {
        referenceTile = input;
        distance = defsaultDistance;
        visited = false;
    }
    
    public List<NodeBase> getValidTiles(int maxDistance, int currentDistance = 0)
    {
        List<NodeBase> tiles = new List<NodeBase>();
        visited = true;
        if (currentDistance == maxDistance) return tiles;

        int nextDistance = currentDistance + 1;

        foreach (Tile t in referenceTile.getNeighbours())
        {
            if(!(t.Walkable))
                continue;
            if (!tiles.Exists(n => n.referenceTile == t)) {
                NodeBase newNode = new NodeBase(t);
                tiles.Add(newNode);
            }

            int index = tiles.FindIndex(n => n.referenceTile == t);

            if (tiles[index].visited) continue;

            if (tiles[index].distance > nextDistance) tiles[index].distance = nextDistance;

            tiles.AddRange(tiles[index].getValidTiles(maxDistance, nextDistance));
        }

        return tiles;
    }

}
