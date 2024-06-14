using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeBase
{
    /// <summary>
    /// Function for calculating distance using Djikstra's algorithm
    /// </summary>
    public Tile referenceTile;
    public bool visited;
    public int distance;
    public NodeBase(Tile input, int defaultDistance = 1000)
    {
        referenceTile = input;
        distance = defaultDistance;
        visited = false;
    }
    
    public List<NodeBase> getValidTiles(int maxDistance, Faction faction, int currentDistance = 0)
    {
        /// Gets all tiles that can be reached by a given unit. This uses Djikstra's algorithm and is recursive
        /// Args:
        ///     int maxDistance: The maximum distance that can be moved
        ///     Faction faction: The faction of the unit
        ///     int currentDistance: The current distance from the unit, defaults 0
        /// Returns:
        ///     List<NodeBase> tiles: A list of all tiles that can be walked to from the current tile in the algorithm - when called by a unit, this will be all tiles walkable by current unit.
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

            if (tiles[index].distance > nextDistance)
            {
                tiles[index].distance = nextDistance;
                tiles[index].visited = false;
            };

            if (tiles[index].visited) continue;

            tiles.AddRange(tiles[index].getValidTiles(maxDistance, faction, nextDistance));
        }

        return tiles;
    }

}

