using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RiverGenerator
{
    public static List<Vector2> generateRiver(Vector2 start, int gridSize, bool longRiver)
    {
        int length = longRiver ? Random.Range(gridSize / 2, gridSize): Random.Range(0, gridSize / 2);

        return generateRiverRoute(start, gridSize, length);
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
