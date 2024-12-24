using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Tile[] tileTypes;
    public static TileManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    public Tile getTile(tileType ident)
    {
        return tileTypes.Where(u => u.thisType == ident).First();
    }

    public Tile[] getAllTiles()
    {
        return tileTypes;
    }
}
