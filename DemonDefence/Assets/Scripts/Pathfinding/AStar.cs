using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    static List<AStarNode> open;
    static List<AStarNode> closed;

    public static (List<AStarNode>, float) AStarPathfinder(Tile origin, Tile destination)
    {
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
                float pathLength = 0;
                while(current.referenceTile != origin)
                {
                    path.Add(current);
                    pathLength += current.g;
                    current = current.parentNode;
                }
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
            Debug.Log(open.Count);
        }

        return (null, 0);
    }

    
}
