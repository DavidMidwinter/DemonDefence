using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{

    public List<Vector2> tiles;
    public List<Vector2> borderTiles;
    protected List<Vector2> requiredBorderTiles;
    protected List<Vector2> requiredTiles;


    public void setTiles(Vector2 origin)
    {
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
    }

    public List<Vector2> getTiles()
    {
        return tiles;
    }

    public List<Vector2> getBorderTiles()
    {
        return borderTiles;
    }
    public List<Vector2> getAllTiles()
    {
        List<Vector2> all_tiles = tiles;
        all_tiles.AddRange(borderTiles);
        return all_tiles;
    }

}
