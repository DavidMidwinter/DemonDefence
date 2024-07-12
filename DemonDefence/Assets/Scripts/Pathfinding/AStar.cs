using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    /// <summary>
    /// Functionality for A* pathfinding
    /// </summary>
    static List<AStarNode> open;
    static List<AStarNode> closed;

    public static (List<AStarNode>, float) AStarPathfinder(Tile origin, Tile destination)
    {
        /// Calculate the shortest distance between two tiles.
        /// Args:
        ///     Tile origin: The tile that we are starting from.
        ///     Tile destination: The tile that we want to reach.
        /// Returns:
        ///     Tuple (List<AstarNode>, float) - the list stores all A* nodes in the path, the float stores the path length.
        Debug.Log("Find Path");
        List<AStarNode> path = new List<AStarNode>();
        open = new List<AStarNode>();
        closed = new List<AStarNode>();

        AStarNode originNode = new AStarNode(origin);
        originNode.h = 0;
        originNode.f = 0;
        open.Add(originNode);

        while(open.Count > 0 && open.Count < System.Math.Pow(GridManager.Instance.getGridSize(), 2))
        {
            var current_node = open[0];
            int current_index = 0;
            foreach(AStarNode item in open)
            {
                if(item.f < current_node.f)
                {
                    current_index = open.FindIndex(n => n == item);
                    current_node = item;
                }
            }
            open.Remove(current_node);
            closed.Add(current_node);

            if (current_node.referenceTile == destination)
            {
                Debug.Log("Found Path");
                AStarNode current = current_node.parentNode;
                while(current.referenceTile != origin)
                {
                    path.Add(current);
                    if (GameManager.Instance.debugMode && current.referenceTile is Ground)
                        (current.referenceTile as Ground)._value_display.text = $"{current.g}";
                    current = current.parentNode;
                }
                float pathLength = (path.Count > 0) ? path[0].g : 0;
                path.Reverse();
                open = null;
                closed = null;
                return (path, pathLength);
            }

            List<AStarNode> childNodes = new List<AStarNode>();
            foreach (Tile t in current_node.referenceTile.getNeighbours())
            {
                if (!t.Walkable && t != destination) continue;

                AStarNode newNode = new AStarNode(t, current_node);
                childNodes.Add(newNode);
            }

            foreach(AStarNode child in childNodes)
            {
                if(closed.Exists(n => n.referenceTile == child.referenceTile))
                {
                    continue;
                }

                child.g = current_node.g + 1;
                child.calculateHeuristic(destination);
                child.f = child.g + child.h;
                int open_check = open.FindIndex(n => n.referenceTile == child.referenceTile);
                
                if (open_check >= 0 && child.g > open[open_check].g) continue;
                
                open.Add(child);
                

            }
        }

        return (null, 0);
    }

    
}
