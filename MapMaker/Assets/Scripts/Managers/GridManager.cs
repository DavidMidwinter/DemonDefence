using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    private GridDataManager gridDataManager;
    TileManager tileManager => TileManager.Instance;
    private Dictionary<Vector2, TileSlot> _tiles;
    [SerializeField] private int _gridSize;
    [SerializeField] private TileSlot _tileSlotPrefab;
    [SerializeField] private string fileName;
    private Vector2[] validNeighbours = { 
        new Vector2 {x=-1, y=0},
        new Vector2 {x=-1, y=-1},
        new Vector2 {x=0, y=-1},
    };
    public List<tileType> notWalkable;
    private Tile defaultTile;
    [SerializeField]
    private List<Building> buildings;
    [SerializeField]
    public SpawnpointObject playerSpawnPrefab;
    [SerializeField]
    private List<SpawnpointObject> playerSpawns;
    [SerializeField]
    public SpawnpointObject enemySpawnPrefab;
    [SerializeField]
    private List<SpawnpointObject> enemySpawns;
    [SerializeField]
    private int spawnRadius;

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if(PaintStartData.getFilename() != null)
        {
            fileName = PaintStartData.getFilename();
        }
        if (PaintStartData.getGridSize() != 0)
        {
            _gridSize = PaintStartData.getGridSize();
        }
        _tiles = new Dictionary<Vector2, TileSlot>();
        defaultTile = tileManager.getTile(tileType.grass);
        if(fileName == "")
        {
            Debug.LogError("No filename specified");
            Application.Quit();
            return;
        }
        gridDataManager = new GridDataManager(fileName);
        playerSpawns = new List<SpawnpointObject>();
        enemySpawns = new List<SpawnpointObject>();
        if (Directory.Exists(gridDataManager.dataStore)) loadGrid();
        
        else generateEmptyGrid();
    }
    public void placeTile(Tile tileToPlace, Vector2 coords)
    {

        Debug.Log($"{coords}");
        TileSlot newTile = Instantiate(_tileSlotPrefab, coords, Quaternion.identity);
        newTile.setLocation(coords);
        _tiles[coords] = newTile;
        foreach (Vector2 neighbourCoords in validNeighbours)
        {
            Vector2 neighbourLocation = coords + neighbourCoords;
            if (_tiles.ContainsKey(neighbourLocation))
            {
                _tiles[neighbourLocation].addNeighbour(newTile);
                newTile.addNeighbour(_tiles[neighbourLocation]);
            }
        }

        newTile.setTileType(tileToPlace);
    }

    public void addSpawn(SpawnpointObject spawn)
    {
        if (spawn.faction == Faction.Enemy) enemySpawns.Add(spawn);
        else if (spawn.faction == Faction.Player) playerSpawns.Add(spawn);
    }

    public TileSlot getTile(Vector2 location)
    {
        if (_tiles.ContainsKey(location)) return _tiles[location];
        else return null;
    }
    public void generateEmptyGrid() {
        buildings = new List<Building>();


        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                Vector2 coords = new Vector2 { x = x, y = y };
                placeTile(defaultTile, coords);

            }
        }


        SpawnpointObject player = Instantiate(playerSpawnPrefab);
        player.initData(new Vector2(0, 0));
        playerSpawns.Add(player);
        SpawnpointObject enemy = Instantiate(enemySpawnPrefab);
        enemy.initData(new Vector2(_gridSize - 1, _gridSize - 1));
        enemySpawns.Add(enemy);

    }

    public void loadGrid() {
        Debug.Log($"Load grid {fileName}");
        buildings = new List<Building>();
        gridDataManager.loadGridData();
        _gridSize = gridDataManager.getGridSize();
        spawnRadius = gridDataManager.getSpawnRadius();

        foreach (GroundTileData groundTile in gridDataManager.GetGroundTileDatas())
        {
            switch (groundTile.variant)
            {
                case (groundTileType.grassTile):
                    placeTile(tileManager.getTile(tileType.grass), groundTile.location);
                    break;
                case (groundTileType.stoneTile):
                    placeTile(tileManager.getTile(tileType.stone), groundTile.location);
                    break;
                case (groundTileType.waterTile):
                    placeTile(tileManager.getTile(tileType.water), groundTile.location);
                    break;
                default:
                    placeTile(defaultTile, groundTile.location);
                    break;
            }
        }
        foreach(Vector2 location in gridDataManager.getGates())
        {
            placeTile(tileManager.getTile(tileType.gate), location);
        }
        foreach (Vector2 location in gridDataManager.getWalls())
        {
            placeTile(tileManager.getTile(tileType.wall), location);
        }
        foreach (Vector2 location in gridDataManager.getBridges())
        {
            placeTile(tileManager.getTile(tileType.bridge), location);
        }
        foreach (FoliageData foliage in gridDataManager.getFoliage())
        {
            Vector2 location = new Vector2(foliage.x, foliage.y);
            switch (foliage.type)
            {
                case 0:
                    placeTile(tileManager.getTile(tileType.tree), location);
                    _tiles[location].setFoliageData(foliage.rotationY, foliage.rotationW, foliage.scale);
                    break;

                case 1:
                    placeTile(tileManager.getTile(tileType.bush), location);
                    _tiles[location].setFoliageData(foliage.rotationY, foliage.rotationW, foliage.scale);
                    break;

                default:
                    continue;
            }
        }

        for(int x = 0; x < _gridSize; x++)
        {
            for(int y = 0; y < _gridSize; y++)
            {
                Debug.Log($"Check {x} {y}");
                Vector2 coords = new Vector2(x, y);
                if (_tiles.ContainsKey(coords)) continue;
                Debug.Log("Place default tile");
                placeTile(defaultTile, coords);
            }
        }
        
        foreach(BuildingData buildingData in gridDataManager.getBuildings())
        {
            Vector2 origin = new Vector2(buildingData.origin_x, buildingData.origin_y);
            buildingType key = (buildingType)buildingData.buildingKey;
            _tiles[origin].placeBuilding(BuildingManager.Instance.getBuilding(key).prefab);
        }

        loadSelectedSpawnData();

        Debug.Log(_tiles.Count);


    }

    public void loadSpawnmap(int index)
    {
        List<Spawnpoint> player = new List<Spawnpoint>();
        foreach (SpawnpointObject spawnpoint in playerSpawns)
        {
            player.Add(spawnpoint.spawnpointData);
            Destroy(spawnpoint.gameObject);
        }
        gridDataManager.storePlayerSpawns(player);
        List<Spawnpoint> enemy = new List<Spawnpoint>();
        foreach (SpawnpointObject spawnpoint in enemySpawns)
        {
            enemy.Add(spawnpoint.spawnpointData);
            Destroy(spawnpoint.gameObject);
        }
        gridDataManager.storeEnemySpawns(enemy);
        gridDataManager.storeSpawnRadius(spawnRadius);


        gridDataManager.selectSpawnMap(index);
        loadSelectedSpawnData();
    }
    public void loadSpawnmap(string name)
    {
        try
        {
            int index = gridDataManager.spawnData.FindIndex(e => e.Item1 == name);
            loadSpawnmap(index);
        }
        catch(Exception e)
        {
            Debug.LogWarning($"Error loading map {name}:\n" +
                $"{e.Message}");
        }
    }
    public void loadSelectedSpawnData()
    {
        foreach (Spawnpoint spawnpoint in gridDataManager.getPlayerSpawns())
        {
            SpawnpointObject player = Instantiate(playerSpawnPrefab);
            player.initData(spawnpoint);
            player.faction = Faction.Player;
            playerSpawns.Add(player);
        }
        foreach (Spawnpoint spawnpoint in gridDataManager.getEnemySpawns())
        {

            SpawnpointObject enemy = Instantiate(enemySpawnPrefab);
            enemy.initData(spawnpoint);
            enemy.faction = Faction.Enemy;
            enemySpawns.Add(enemy);
        }
    }

    public int getGridSize()
    {
        return _gridSize;
    }

    public void saveMap()
    {
        gridDataManager.cleanData();
        List<Spawnpoint> player = new List<Spawnpoint>();
        foreach (SpawnpointObject spawnpoint in playerSpawns)
            player.Add(spawnpoint.spawnpointData);
        gridDataManager.storePlayerSpawns(player);
        List<Spawnpoint> enemy = new List<Spawnpoint>();
        foreach (SpawnpointObject spawnpoint in enemySpawns)
            enemy.Add(spawnpoint.spawnpointData);
        gridDataManager.storeEnemySpawns(enemy);
        gridDataManager.storeSpawnRadius(spawnRadius);


        foreach (Vector2 location in _tiles.Keys)
        {
            if (_tiles[location].hasBuilding()) continue;
            Debug.Log($"Save {location}");
            switch (_tiles[location].getType())
            {
                case tileType.grass:
                    gridDataManager.storeGroundTile(location, groundTileType.grassTile);
                    break;
                case tileType.stone:
                    gridDataManager.storeGroundTile(location, groundTileType.stoneTile);
                    break;
                case tileType.water:
                    gridDataManager.storeGroundTile(location, groundTileType.waterTile);
                    break;
                case tileType.wall:
                    gridDataManager.storeWall(location);
                    break;
                case tileType.gate:
                    gridDataManager.storeGate(location);
                    break;
                case tileType.tree:
                    storeFoliage(location, _tiles[location], 0);
                    break;
                case tileType.bush:
                    storeFoliage(location, _tiles[location], 1);
                    break;
                case tileType.bridge:
                    gridDataManager.storeBridge(location);
                    break;
                default:
                    break;
            }
        }

        List<BuildingData> buildingData = new List<BuildingData>();
        foreach(Building building in buildings)
        {
            BuildingData record = new BuildingData();
            record.buildingName = building.getName();
            record.origin_x = building.getOrigin().x;
            record.origin_y = building.getOrigin().y;
            record.buildingKey = building.getKey();

            buildingData.Add(record);
        }
        gridDataManager.storeGridSize(_gridSize);
        gridDataManager.storeBuildings(buildingData);
        gridDataManager.saveGridData();
    }

    void storeFoliage(Vector2 location, TileSlot tile, int foliageType)
    {
        (float rotationY, float rotationW, float scale) foliageInfo = tile.getFoliageData();

        gridDataManager.storeFoliage((location, foliageInfo.rotationY, foliageInfo.rotationW, foliageInfo.scale, foliageType));

    }

    public void addBuilding(Building building)
    {
        buildings.Add(building);
    }
    public void removeBuilding(Building building)
    {
        buildings.Remove(building);
    }

    public SpawnpointObject getSpawn(Faction faction)
    {
        switch (faction)
        {
            case Faction.Enemy:
                return enemySpawns[0];
            case Faction.Player:
                return playerSpawns[0];
            default:
                return null;
        }
    }
    public List<SpawnpointObject> getSpawns(Faction faction)
    {
        switch (faction)
        {
            case Faction.Enemy:
                return enemySpawns;
            case Faction.Player:
                return playerSpawns;
            default:
                return null;
        }
    }

    public int getSpawnRadius()
    {
        return spawnRadius;
    }


    public void removeSpawn(SpawnpointObject spawn)
    {
        if (spawn.faction == Faction.Enemy) {
            enemySpawns.Remove(spawn);
        }
        else if (spawn.faction == Faction.Player) {
            playerSpawns.Remove(spawn); 
        }
    }


}