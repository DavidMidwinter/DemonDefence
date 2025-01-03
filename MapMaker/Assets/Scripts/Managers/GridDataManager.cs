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
    public string dataStore;
    public string saveDirectory => Utils.saveDirectory;
    public GridData gridData = new GridData();
    public List<(string, SpawnData)> spawnData = new List<(string, SpawnData)>();
    public int selectedSpawnData = 0;

    public GridDataManager(string filename)
    {

        dataStore = Path.Combine(saveDirectory, $"{filename}");
        spawnData = new List<(string, SpawnData)>();
    }

    public void saveGridData()
    {
        Directory.CreateDirectory(saveDirectory);
        Directory.CreateDirectory(dataStore);
        Directory.CreateDirectory(spawnMapsLocation());

        string gridDataString = JsonUtility.ToJson(gridData, true);
        File.WriteAllText(tileMapLocation(), gridDataString);
        string mapFile;
        foreach ((string, SpawnData) map in spawnData)
        {
            mapFile = JsonUtility.ToJson(map.Item2, true);
            File.WriteAllText(Path.Combine(spawnMapsLocation(), map.Item1), mapFile);

        }
    }
    public void loadGridData()
    {
        gridData = JsonUtility.FromJson<GridData>(File.ReadAllText(tileMapLocation()));
        foreach(string spawnfile in Directory.EnumerateFiles(spawnMapsLocation()))
        {
            Debug.Log(spawnfile);
            if (spawnfile.Contains(".meta"))
            {
                Debug.Log($"{spawnfile} is a metafile and cannot be loaded");
                continue; 
            }
            spawnData.Add((Path.GetFileName(spawnfile), JsonUtility.FromJson<SpawnData>(File.ReadAllText(spawnfile))));
            Debug.Log($"Loaded {spawnfile}");
        }
    }

    private string tileMapLocation()
    {
        return Path.Combine(dataStore, "tilemap.json");
    }
    private string spawnMapsLocation()
    {
        return Path.Combine(dataStore, "spawnmaps");
    }

    public void cleanData()
    {
        gridData.cleanData();
    }
    public void storeSpawnRadius(int value)
    {
        spawnData[selectedSpawnData].Item2.spawnRadius = value;
    }

    public void storeEnemySpawns(List<Spawnpoint> spawnLocations)
    {
        spawnData[selectedSpawnData].Item2.storeEnemySpawns(spawnLocations);
    }

    public void storePlayerSpawns(List<Spawnpoint> spawnLocations)
    {
        spawnData[selectedSpawnData].Item2.storePlayerSpawns(spawnLocations);
    }

    public void storeCitySize(int radius)
    {
        gridData.storeCitySize(radius);
    }
    public void storeBuildings(List<BuildingData> placedBuildings)
    {
        gridData.storeBuildings(placedBuildings);
    }

    public void storeGridSize(int size)
    {
        gridData.storeGridSize(size);
    }

    public void storeCoreBuilding(BuildingData building)
    {
        gridData.storeCoreBuilding(building);
    }

    public void storeFoliage((Vector2 location, float rotation, float rotationW, float scale, int type) p)
    {
        gridData.storeFoliage(p);
    }

    public void storeGroundTile(Vector2 location, groundTileType variant)
    {
        gridData.storeGroundTile(location, variant);
    }

    public void storeWall(Vector2 location)
    {
        gridData.storeWall(location);
    }
    public void storeGate(Vector2 location)
    {
        gridData.storeGate(location);
    }
    public void storeBridge(Vector2 location)
    {
        gridData.storeBridge(location);
    }

    public List<BuildingData> getBuildings()
    {
        return gridData._buildings;
    }

    public List<FoliageData> getFoliage()
    {
        return gridData._foliage;
    }

    public List<GroundTileData> GetGroundTileDatas()
    {
        return gridData._groundTiles;
    }

    public List<Vector2> getWalls()
    {
        return gridData._wallTiles;
    }
    public List<Vector2> getGates()
    {
        return gridData._gateTiles;
    }
    public List<Vector2> getBridges()
    {
        return gridData._bridgeTiles;
    }

    public int getGridSize()
    {
        return gridData.gridSize;
    }

    public int getCitySize()
    {
        return gridData.citySize;
    }

    public bool getIsWalled()
    {
        return gridData.isWalled;
    }

    public int getSpawnRadius()
    {
        return spawnData[selectedSpawnData].Item2.spawnRadius;
    }
    
    public List<Spawnpoint> getPlayerSpawns()
    {
        return spawnData[selectedSpawnData].Item2.playerSpawnLocations;
    }

    public List<Spawnpoint> getEnemySpawns()
    {
        return spawnData[selectedSpawnData].Item2.enemySpawnLocations;
    }

    public void selectSpawnMap(int select)
    {
        selectedSpawnData = select;
    }

    public int createNewSpawnMap()
    {
        string name = $"spawnmap_{spawnData.Count}.json";
        spawnData.Add((name, new SpawnData()));
        return spawnData.Count - 1;
    }
}

[System.Serializable]
public class SpawnData
{
    public int spawnRadius;
    public List<Spawnpoint> playerSpawnLocations;
    public List<Spawnpoint> enemySpawnLocations;
    
    void Awake()
    {
        cleanData();
    }

    public void cleanData()
    {
        playerSpawnLocations = new List<Spawnpoint>();
        enemySpawnLocations = new List<Spawnpoint>();
    }
    public void storeEnemySpawns(List<Spawnpoint> spawnLocations)
    {
        enemySpawnLocations = spawnLocations;
    }
    public void storePlayerSpawns(List<Spawnpoint> spawnLocations)
    {
        playerSpawnLocations = spawnLocations;
    }

}


[System.Serializable]
public class GridData
{
    public int gridSize;
    public int citySize;
    public bool isWalled;
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

    public void storeCitySize(int radius)
    {
        citySize = radius;
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
