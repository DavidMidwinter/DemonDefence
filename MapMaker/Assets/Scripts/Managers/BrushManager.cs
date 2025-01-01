using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BrushManager : MonoBehaviour
{
    public static BrushManager Instance;

    public static event Action onBrushStateChanged;
    TileManager tileManager => TileManager.Instance;
    BuildingManager buildingManager => BuildingManager.Instance;
    public Tile selectedTile;
    public BuildingTemplate selectedBuilding;
    public SpawnpointObject selectedSpawn;
    public SpawnpointObject selectedToEdit;
    public brushState state;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        selectedTile = tileManager.getTile(tileType.stone);
        selectedBuilding = buildingManager.getBuilding(buildingType.building1x2);
        selectedSpawn = GridManager.Instance.playerSpawnPrefab;
    }

    public void setBrush(brushState newState)
    {
        state = newState;
        onBrushStateChanged?.Invoke();
    }

    public void selectSpawn(SpawnpointObject selected)
    {
        selectedToEdit = selected;
        TileSlot.callTileCheck();
    }
    public void clearSpawnSelect()
    {
        selectSpawn(null);
    }

}

public enum brushState
{
    paintTiles,
    placeBuilding,
    deleteBuilding,
    placeCoreBuilding,
    placeSpawnpoint,
    deleteSpawnpoint,
    editSpawnpoint
}
