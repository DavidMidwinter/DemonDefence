using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    /// <summary>
    /// Basic functionality shared by all buildings
    /// </summary>
    [SerializeField] public string buildingName;
    public List<Vector2> tiles;
    public List<Vector2> borderTiles;
    public List<Vector2> all_tiles;
    protected List<Vector2> requiredBorderTiles;
    protected List<Vector2> requiredTiles;


    public void setTiles(Vector2 origin)
    {
        /// Set the border and regular tiles for this building
        /// Args:
        ///     Vector2 origin: The origin point for this building
        tiles = new List<Vector2>();
        for (int x = 0; x < requiredTiles.Count; x++){
            tiles.Add(origin + requiredTiles[x]);
            
        }

        borderTiles = new List<Vector2>();
        for (int y = 0; y < requiredBorderTiles.Count; y++)
        {
            Debug.Log($"{y}:{origin + requiredBorderTiles[y]}");
            borderTiles.Add(origin + requiredBorderTiles[y]);
        }

        all_tiles = new List<Vector2>();
        all_tiles.AddRange(tiles);
        all_tiles.AddRange(borderTiles);
    }

    public List<Vector2> getTiles()
    {
        /// Return the list of tiles this building is occupying
        return tiles;
    }

    public List<Vector2> getBorderTiles()
    {
        /// Return the list of tiles bordering this building
        return borderTiles;
    }
    public List<Vector2> getAllTiles()
    {
        /// Return all tiles this building is either sitting on or bordered by
        return all_tiles;
    }

    public void generateRequiredTiles((int x, int y) footprintSize, (int x, int y) bufferSize, (int x, int y) bufferOrigin)
    {
        /// Calculate the tile templates for this building - this includes the tiles the building sits on, and the tiles bordering it on the top and right faces.
        /// Args:
        ///     (int x, int y) footprintSize: The size of the building itself
        ///     (int x, int y) bufferSize: The size of the border
        ///     (int x, int y) bufferOrigin: Where to start calculating the border.
        requiredTiles = new List<Vector2>();
        for (int x = 0; x < footprintSize.x; x++)
        {
            for (int y = 0; y < footprintSize.y; y++)
            {
                requiredTiles.Add(new Vector2(x, y));
            }
        }
        requiredBorderTiles = new List<Vector2>();

        for (int x = bufferOrigin.x; x < (bufferSize.x + bufferOrigin.x); x++)
        {
            for (int y = bufferOrigin.y; y < (bufferSize.y + bufferOrigin.y); y++)
            {
                var tile = new Vector2(x, y);
                if (!requiredTiles.Contains(tile))
                {
                    requiredBorderTiles.Add(tile);
                }
            }
        }
        log_tiles(requiredTiles);
        log_tiles(requiredBorderTiles);

    }

    public void log_tiles(List<Vector2> log_tiles)
    {
        /// Log all vectors from list
        /// Args:
        ///     List<Vector2> log_tiles: The Vector2s to log.
        string output = "";
        foreach (Vector2 tile in log_tiles)
        {
            output = output + $"({tile.x}{tile.y}) ";
        }
        Debug.Log(output);
    }
}
