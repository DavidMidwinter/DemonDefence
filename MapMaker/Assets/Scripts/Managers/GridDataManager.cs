using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class Spawnpoint
{
    public Vector2 location;
    public List<UnitType> validUnits;

    public Spawnpoint(Vector2 spawnLocation, List<UnitType> unitTypes = null)
    {
        location = spawnLocation;
        validUnits = (unitTypes is not null) ? unitTypes : new List<UnitType>();
    }
}
public class GridDataManager
{
    public string saveFile;
    public string saveDirectory = Path.Combine(Application.dataPath, "Maps");
    public GridData data = new GridData();

    public GridDataManager(string filename)
    {

        saveFile = Path.Combine(saveDirectory, $"{filename}.json");
    }

    public void saveGridData()
    {
        Directory.CreateDirectory(saveDirectory);
        string gridDataString = JsonUtility.ToJson(data, true);

        File.WriteAllText(saveFile, gridDataString);
    }

    public void loadGridData()
    {
        string gridDataString = File.ReadAllText(saveFile);
        data = JsonUtility.FromJson<GridData>(gridDataString);
    }
}

[System.Serializable]
public class GridData
{
    public int gridSize;
    public int spawnRadius;
    public int citySize;
    public bool isWalled;
    public bool spreadSpawns;
    public List<Spawnpoint> playerSpawnLocations;
    public List<Spawnpoint> enemySpawnLocations;
    public BuildingData coreBuilding;
    public List<BuildingData> _buildings;
    public List<FoliageData> _foliage;
    public List<GroundTileData> _groundTiles;
    public List<Vector2> _wallTiles;
    public List<Vector2> _gateTiles;
    public List<Vector2> _bridgeTiles;

    public void cleanData()
    {
        _groundTiles = new List<GroundTileData>();
        _foliage = new List<FoliageData>();
        _wallTiles = new List<Vector2>();
        _gateTiles = new List<Vector2>();
        _bridgeTiles = new List<Vector2>();
        _buildings = new List<BuildingData>();

    }
    public void storeSpawnRadius(int radius)
    {
        spawnRadius = radius;
    }
    public void storeCitySize(int radius)
    {
        citySize = radius;
    }

    public void storeEnemySpawns(List<Spawnpoint> spawnLocations)
    {
        enemySpawnLocations = spawnLocations;
    }
    public void storePlayerSpawns(List<Spawnpoint> spawnLocations)
    {
        playerSpawnLocations = spawnLocations;
    }
    public void storeBuildings(List<BuildingData> placedBuildings)
    {
        _buildings = placedBuildings;
    }

    public void storeGridSize(int size)
    {
        gridSize = size;
    }

    public void storeCoreBuilding(BuildingData building)
    {
        coreBuilding = building;
    }

    public void storeFoliage((Vector2 location, float rotation, float rotationW, float scale, int type) p)
    {

        if (_foliage == null)
        {
            _foliage = new List<FoliageData>();
        }

        _foliage.RemoveAll(u => u.x == p.location.x && u.y == p.location.y);

        FoliageData newFoliage = new FoliageData();
        newFoliage.x = p.location.x;
        newFoliage.y = p.location.y;
        newFoliage.rotationY = p.rotation;
        newFoliage.rotationW = p.rotationW;
        newFoliage.scale = p.scale;
        newFoliage.type = p.type;
        _foliage.Add(newFoliage);
    }

    public void storeGroundTile(Vector2 location, groundTileType variant)
    {
        if (_groundTiles == null) _groundTiles = new List<GroundTileData>();
        _groundTiles.Add(new GroundTileData(location, variant));
    }

    public void storeWall(Vector2 location)
    {
        if (_wallTiles == null) _wallTiles = new List<Vector2>();
        _wallTiles.RemoveAll(u => u.x == location.x && u.y == location.y);
        _wallTiles.Add(location);
    }
    public void storeGate(Vector2 location)
    {
        if (_gateTiles == null) _gateTiles = new List<Vector2>();
        _gateTiles.RemoveAll(u => u.x == location.x && u.y == location.y);
        _gateTiles.Add(location);
    }
    public void storeBridge(Vector2 location)
    {
        if (_bridgeTiles == null) _bridgeTiles = new List<Vector2>();
        _bridgeTiles.RemoveAll(u => u.x == location.x && u.y == location.y);
        _bridgeTiles.Add(location);
    }
}

[System.Serializable]
public class BuildingData
{
    public string buildingName;
    public float origin_x;
    public float origin_y;
    public int buildingKey;

}

[System.Serializable]
public class FoliageData
{
    public float x;
    public float y;
    public float rotationY;
    public float rotationW;
    public float scale;
    public int type;
}

[Serializable]
public class GroundTileData
{
    public Vector2 location;
    public groundTileType variant;

    public GroundTileData(Vector2 coords, groundTileType tileType)
    {
        location = coords;
        variant = tileType;
    }

}

public enum tileType
{
    grass,
    stone,
    water,
    wall,
    gate,
    tree,
    bush,
    bridge
}

public enum groundTileType
{
    stoneTile,
    grassTile,
    waterTile
}


public enum UnitType
{
    Common,
    Pious,
    Mechanised,
    Cultist,
    Demonic,
    Despoiler,
    Leader
}
