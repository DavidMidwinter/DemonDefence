using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushManager : MonoBehaviour
{
    public static BrushManager Instance;
    TileManager tileManager => TileManager.Instance;
    public Tile selectedTile;
    public BuildingTemplate selectedBuilding;
    public brushState state;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        selectedTile = tileManager.getTile(tileType.stone);
    }



}

public enum brushState
{
    paintTiles,
    placeBuilding,
    placeCoreBuilding
}
