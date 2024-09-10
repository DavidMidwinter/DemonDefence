using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RiverGenerator
{
    public static List<Vector2> generateRiver(Vector2 start, int gridSize, bool longRiver)
    {
        int length = longRiver ? Random.Range(gridSize / 2, gridSize): Random.Range(0, gridSize / 2);

        List<Vector2> route = generateRiverRoute(start, gridSize, length);

        List<Vector2> river = new List<Vector2>(route);

        foreach(Vector2 location in route)
        {
            for(int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 point = new Vector2(location.x + x, location.y + y);
                    Debug.LogWarning(point);
                    if (point.x < 0 || point.x > gridSize || point.y < 0 || point.y > gridSize) continue;
                    if (route.Contains(point)) continue;
                    river.Add(point);
                }
            }
        }
        
        return river;
    }

    public static List<Vector2> generateRiverRoute(Vector2 start, int gridSize, int length)
    {
        List<Vector2> riverTiles = new List<Vector2>();


        Vector2 currentTile = start;

        while (riverTiles.Count < length)
        {
            if (!riverTiles.Contains(currentTile))
                riverTiles.Add(currentTile);

            currentTile.x += Random.Range(-1, 1);
            currentTile.y += Random.Range(-1, 1);
            if ((currentTile.x < 0
                || currentTile.x >= gridSize)
                || (currentTile.y < 0
                || currentTile.y >= gridSize)
                )
            {
                break;
            }

        }

        return riverTiles;

    }
}
