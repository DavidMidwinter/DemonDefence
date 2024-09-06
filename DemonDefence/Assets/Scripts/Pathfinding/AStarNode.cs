using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    /// <summary>
    /// Node for use in the A* algorithm
    /// </summary>
    public Tile referenceTile;
    public AStarNode parentNode;
    public bool visited;
    public float g;
    public float h;
    public float f;

    public AStarNode(Tile tile, AStarNode parent = null)
    {
        referenceTile = tile;
        parentNode = parent;
    }

    public void calculateHeuristic(Tile destination)
    {
        /// Calculate the heuristic distance to a given tile
        h = Mathf.Pow(destination.transform.position.x - referenceTile.transform.position.x, 2)
            + Mathf.Pow(destination.transform.position.z - referenceTile.transform.position.z, 2);
    }
}
