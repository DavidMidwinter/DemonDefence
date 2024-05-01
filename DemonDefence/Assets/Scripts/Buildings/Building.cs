using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{

    public List<Vector2> tiles;
    protected List<Vector2> requiredTiles;


    public void setTiles(Vector2 origin)
    {
        tiles = new List<Vector2>();
        for (int x = 0; x < requiredTiles.Count; x++){
            tiles.Add(origin + requiredTiles[x]);
        }
    }

    public List<Vector2> getTiles()
    {
        return requiredTiles;
    }    

}
