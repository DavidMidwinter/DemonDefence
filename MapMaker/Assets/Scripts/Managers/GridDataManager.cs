using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

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
public class SpawnLocation
{
    public int x;
    public int y;

    public SpawnLocation(int X, int Y)
    {
        x = X;
        y = Y;
    }
}

[System.Serializable]
public class GridData
{
    public int gridSize;
    public int spawnRadius;
    public int citySize;
    public bool isWalled;
    public SpawnLocation playerSpawn;
    public SpawnLocation enemySpawn;
    public bool spreadSpawns;
    public List<EnemySpawn> enemySpawnLocations;
    public BuildingData coreBuilding;
    public List<BuildingData> _buildings;
    public List<FoliageData> _foliage;
    public List<GroundTileData> _groundTiles;
    public List<Vector2> _wallTiles;
    public List<Vector2> _gateTiles;

    public void storeSpawnRadius(int radius)
    {
        spawnRadius = radius;
    }
    public void storeCitySize(int radius)
    {
        citySize = radius;
    }
    public void storePlayerSpawn(Vector2 location)
    {
        playerSpawn = new SpawnLocation((int)location.x, (int)location.y);
    }
    public void storeEnemySpawn(Vector2 location)
    {
        enemySpawn = new SpawnLocation((int)location.x, (int)location.y);
    }

    public void storeEnemySpawns(bool spreadSpawnLocations, List<EnemySpawn> spawnLocations)
    {
        spreadSpawns = spreadSpawnLocations;
        enemySpawnLocations = spawnLocations;
    }

    public Vector2 getPlayerSpawn()
    {
        return new Vector2(playerSpawn.x, playerSpawn.y);
    }
    public Vector2 getEnemySpawn()
    {
        return new Vector2(enemySpawn.x, enemySpawn.y);
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
        FoliageData newFoliage = new FoliageData();
        newFoliage.x = p.location.x;
        newFoliage.y = p.location.y;
        newFoliage.rotationY = p.rotation;
        newFoliage.rotationW = p.rotationW;
        newFoliage.scale = p.scale;
        newFoliage.type = p.type;

        if (_foliage == null)
        {
            _foliage = new List<FoliageData>();
        }
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
        _wallTiles.Add(location);
    }
    public void storeGate(Vector2 location)
    {
        if (_gateTiles == null) _gateTiles = new List<Vector2>();
        _gateTiles.Add(location);
    }
}

[System.Serializable]
public class EnemySpawn
{
    public Vector2 location;
    public List<UnitType> validUnits;

    public EnemySpawn(Vector2 spawnLocation, List<UnitType> unitTypes = null)
    {
        location = spawnLocation;
        validUnits = (unitTypes is not null) ? unitTypes : new List<UnitType>();
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
    bush
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
