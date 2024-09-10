using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class RiverGenerator
{
    private static List<Vector2> quarters = new List<Vector2> {
        new Vector2(1, 1),
        new Vector2(1, -1),
        new Vector2(-1, 1),
        new Vector2(-1, -1)
    };
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

    public static List<Vector2> generateRivers(int gridSize, int numberOfRivers)
    {
        Vector2 center = new Vector2(gridSize / 2, gridSize / 2);
        List<Vector2> riverTiles = new List<Vector2>();
        quarters = quarters.OrderBy(s => Random.value).ToList();
        int quarter = 0;
        for (int i = 0; i < numberOfRivers; i++)
        {
            Vector2 origin = new Vector2();
            do
            {
                origin.x = Random.Range(2, gridSize/2);
                origin.y = Random.Range(2, gridSize/2);
                origin *= quarters[quarter];
                origin = center - origin;
            } while (riverTiles.Contains(origin));
            riverTiles.AddRange(generateRiver(origin, gridSize, true));

            quarter++;
            if (quarter >= quarters.Count) quarter = 0;
        }

        return riverTiles;
    }
}
