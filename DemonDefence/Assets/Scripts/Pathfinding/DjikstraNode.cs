using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DjikstraNode
{
    /// <summary>
    /// Function for calculating distance using Djikstra's algorithm
    /// </summary>
    public Tile referenceTile;
    public bool visited;
    public float distance;
    public DjikstraNode parent = null;
    public DjikstraNode(Tile input, float defaultDistance = 1000)
    {
        referenceTile = input;
        distance = defaultDistance;
        visited = false;
    }
    
    public List<DjikstraNode> getValidTiles(int maxDistance, Faction faction, float currentDistance = 0, List<DjikstraNode> tiles = null)
    {
        /// Gets all tiles that can be reached by a given unit. This uses Djikstra's algorithm and is recursive
        /// Args:
        ///     int maxDistance: The maximum distance that can be moved
        ///     Faction faction: The faction of the unit
        ///     int currentDistance: The current distance from the unit, defaults 0
        /// Returns:
        ///     List<DjikstraNode> tiles: A list of all tiles that can be walked to from the current tile in the algorithm - when called by a unit, this will be all tiles walkable by current unit.
        if(tiles == null)
            tiles = new List<DjikstraNode>();
        visited = true;
        if (GameManager.Instance.debugMode && referenceTile is Ground)
            (referenceTile as Ground)._value_display.text = $"{distance}";
        if (currentDistance >= maxDistance) return tiles;

        foreach (Tile t in referenceTile.getNeighbours())
        {

            if(!(t.Walkable))
                continue;

            float nextDistance = currentDistance + Utils.getDistanceIncrease(referenceTile, t);
            if (nextDistance > maxDistance) 
                continue;

            if (!tiles.Exists(n => n.referenceTile == t)) {
                DjikstraNode newNode = new DjikstraNode(t);
                tiles.Add(newNode);
            }

            int index = tiles.FindIndex(n => n.referenceTile == t);


            if (tiles[index].distance > nextDistance)
            {
                tiles[index].distance = nextDistance;
                tiles[index].visited = false;
                tiles[index].parent = this;
            };

            if (tiles[index].visited) continue;

            tiles = tiles[index].getValidTiles(maxDistance, faction, nextDistance, tiles);
        }

        return tiles;
    }

}

