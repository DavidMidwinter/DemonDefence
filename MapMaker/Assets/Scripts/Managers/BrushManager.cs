using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushManager : MonoBehaviour
{
    public static BrushManager Instance;
    TileManager tileManager => TileManager.Instance;
    BuildingManager buildingManager => BuildingManager.Instance;
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
        selectedBuilding = buildingManager.getBuilding(buildingType.building1x2);
    }



}

public enum brushState
{
    paintTiles,
    placeBuilding,
    deleteBuilding,
    placeCoreBuilding
}
