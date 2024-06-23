using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// All functionality relating to the set up of the grid
    /// </summary>
    /// 


    [SerializeField] private int _gridSize;
    [SerializeField] private string coreType;
    [SerializeField] private int _maxBuildings = -1;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _buildingTilePrefab;
    [SerializeField] private Dictionary<Vector2, Tile> _tiles;
    [SerializeField] private bool saveToFile;
    [SerializeField] private bool loadFromFile;
    [SerializeField] private string fileName;
    private GridDataManager gridDataManager;
    public CameraController cameraObject;
    public Building coreBuilding;
    [SerializeField] private BuildingRegister register;
    public static GridManager Instance;
    private Vector2[] validNeighbours = {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };
    [SerializeField] private int spawnRadius;
    private Vector2 playerSpawn;
    private Vector2 enemySpawn;


    void Awake()
    {
        Instance = this;
        gridDataManager = new GridDataManager(fileName);
        Debug.Log(Application.dataPath);
    }


    public void GenerateGrid()
    {

        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();

        if (loadFromFile) loadExistingGrid();
        else generateRandomGrid();

        cameraObject.Init(_gridSize, 10);
        GameManager.Instance.UpdateGameState(GameState.SpawnPlayer);
    }


    void loadExistingGrid()
    {
        /// Load an existing grid from a JSon file
        Debug.Log($"Load grid {fileName}");
        gridDataManager.loadGridData();
        _gridSize = gridDataManager.data.gridSize;
        playerSpawn = gridDataManager.data.getPlayerSpawn();
        enemySpawn = gridDataManager.data.getEnemySpawn();
        spawnRadius = gridDataManager.data.spawnRadius;

        if (gridDataManager.data.coreBuilding != null)
        {
            Vector2 location = new Vector2(gridDataManager.data.coreBuilding.origin_x, gridDataManager.data.coreBuilding.origin_y);

            Building buildingToPlace = Instantiate(register.getCoreBuilding(gridDataManager.data.coreBuilding.buildingName),
                        vector2to3(location) * 10, Quaternion.identity);

            buildingToPlace.setTiles(location);

            buildingToPlace.name = $"{gridDataManager.data.coreBuilding.buildingName} {location.x} {location.y}";

            placeBuilding(buildingToPlace);
        }

        foreach (BuildingData building in gridDataManager.data._buildings)
        {
            Vector2 location = new Vector2(building.origin_x, building.origin_y);

            Building buildingToPlace = Instantiate(register.get_specific_building_by_key(building.buildingName),
                        vector2to3(location) * 10, Quaternion.identity);

            buildingToPlace.setTiles(location);

            buildingToPlace.name = $"Building {location.x} {location.y}";

            placeBuilding(buildingToPlace);

        }

        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector2 location = new Vector2(x, z);
                if (_tiles.ContainsKey(location))
                {
                    continue;
                }
                placeTile(_tilePrefab, location);
            }
        }
    }

    void generateRandomGrid()
    {
        /// Generate a new grid
        Debug.Log("Create new grid");
        playerSpawn = new Vector2(spawnRadius, spawnRadius);
        enemySpawn = new Vector2(_gridSize - spawnRadius, _gridSize - spawnRadius);
        int existingBuildings = 0;
        List<BuildingData> buildings = new List<BuildingData>();
        Building coreTemplate = register.getCoreBuilding(coreType);
        if (coreTemplate)
        {
            int centre = (int)_gridSize / 2 - 1;
            Vector2 core_location = new Vector2(centre, centre);
            coreBuilding = Instantiate(coreTemplate, vector2to3(core_location) * 10, Quaternion.identity);
            coreBuilding.setTiles(core_location);
            coreBuilding.name = $"Church {core_location.x} {core_location.y}";
            placeBuilding(coreBuilding);

            BuildingData coreBuildingData = new BuildingData();
            coreBuildingData.buildingName = coreType;
            coreBuildingData.origin_x = core_location.x;
            coreBuildingData.origin_y = core_location.y;
            coreBuildingData.buildingKey = 0;
            gridDataManager.data.storeCoreBuilding(coreBuildingData);
        }



        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector2 location = new Vector2(x, z);
                if (_tiles.ContainsKey(location))
                {
                    continue;
                }



                if ((_maxBuildings == -1 || existingBuildings < _maxBuildings)
                    && UnityEngine.Random.Range(0, 5) == 3)
                {
                    int buildingKey = register.get_random_building();

                    Building buildingToPlace = Instantiate(register.get_specific_building(buildingKey),
                        vector2to3(location) * 10, Quaternion.identity);

                    buildingToPlace.setTiles(location);

                    buildingToPlace.name = $"Building {location.x} {location.y}";

                    if (evaluateBuildingPlacement(buildingToPlace))
                    {
                        placeBuilding(buildingToPlace);
                        existingBuildings += 1;

                        BuildingData buildingData = new BuildingData();
                        buildingData.buildingName = buildingToPlace.buildingName;
                        buildingData.origin_x = location.x;
                        buildingData.origin_y = location.y;
                        buildingData.buildingKey = buildingKey;

                        buildings.Add(buildingData);
                    }
                    else
                    {
                        Debug.Log("Building cannot be placed here");
                        placeTile(_tilePrefab, location);
                        Destroy(buildingToPlace.gameObject);
                    }
                }
                else
                {
                    placeTile(_tilePrefab, location);
                }

            }
        }
        if (saveToFile)
        {
            gridDataManager.data.storeSpawnRadius(spawnRadius);
            gridDataManager.data.storePlayerSpawn(playerSpawn);
            gridDataManager.data.storeEnemySpawn(enemySpawn);
            gridDataManager.data.storeBuildings(buildings);
            gridDataManager.data.storeGridSize(_gridSize);
            gridDataManager.saveGridData();
        }
    }

    void placeTile(Tile tileToPlace, Vector2 location)
    {
        /// Place a tile of type 'tileToPlace' at location 'location'
        /// Args:
        ///     Tile tileToPlace: The tile to place
        ///     Vector2 location: The location to place the tile at - this is a vector 2, storing the 'x' and 'z' coords
        var spawnedTile = Instantiate(tileToPlace, vector2to3(location) * 10, Quaternion.identity);
        spawnedTile.name = $"Tile {location.x} {location.y}";

        spawnedTile.Init(vector2to3(location));
        _tiles[location] = spawnedTile;

        foreach (Vector2 t in validNeighbours)
        {
            Vector2 neighbourLocation = location + t;
            if (_tiles.ContainsKey(neighbourLocation))
            {
                _tiles[neighbourLocation].setNeighbour(spawnedTile);
                spawnedTile.setNeighbour(_tiles[neighbourLocation]);
            }
        }
    }


    Vector3 vector2to3(Vector2 vector)
    {
        /// Convert a vector 2 to a vector 3. This specifically sets the new vector3 to 'x, 0, y'
        /// Args:
        ///     Vector2 vector: The vector to convert
        /// Returns:
        ///     The vector as a Vector3 object
        return new Vector3(vector.x, 0, vector.y);
    }

    bool evaluateBuildingPlacement(Building buildingToEvaluate)
    {
        /// Check if a building is able to be placed.
        /// A building cannot be placed if it:
        /// 1. Runs off the edge of the board
        /// 2. Collides with an existing tile (tiles are placed originating from the bottom left)
        /// 3. Collides with either the Player or Enemy Spawn Zones
        /// Args:
        ///     Building buildingToEvaluate: The building object to place
        /// Returns:
        ///     True if the building can be placed; False if otherwise
        foreach (Vector2 t in buildingToEvaluate.getAllTiles())
        {
            if (t.x >= _gridSize || t.y >= _gridSize)
            {

                return false;
            }

            if (_tiles.ContainsKey(t))
            {
                return false;
            }
            if (checkIsEnemySpawn(t) || checkIsPlayerSpawn(t)) return false;

        }
        return true;
    }

    void placeBuilding(Building buildingToPlace)
    {
        /// Place a Building on the grid.
        /// Args:
        ///     Building buildingToPlace: The building to place on the grid.
        foreach (Vector2 t in buildingToPlace.getTiles())
        {
            placeTile(_buildingTilePrefab, t);

        }
        foreach (Vector2 t in buildingToPlace.getBorderTiles())
        {
            if (t.x < _gridSize && t.y < _gridSize)
            {
                placeTile(_tilePrefab, t);
            }
        }
    }

    public Tile GetPlayerSpawnTile()
    {
        /// Get a random tile that a Player Unit can spawn on.
        /// This is a tile that:
        /// 1. Is in the Player's Spawn Zone.
        /// 2. Is currently able to be walked on.
        /// Returns
        ///     Tile: A random tile;
        ///     Null if no valid tile can be found
        try
        {
            return _tiles.Where(
                t => (checkIsPlayerSpawn(t.Key))
                && t.Value.Walkable).
                OrderBy(t => UnityEngine.Random.value).First().Value;
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("No tile available");
            return null;
        }
    }

    public Tile GetNearestTile(Tile origin, int minimumDistance = 0)
    {
        try
        {
            return _tiles.Where(
                t => t.Value.Walkable && 
                origin.getDistance(t.Value) >= minimumDistance * 10
                && !t.Value.getNeighbours().Any(u => u.occupiedUnit != null)
                ).
                OrderBy(t => origin.getDistance(t.Value)).First().Value;
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("No tile available");
            return null;
        }
    }
    public bool checkIsPlayerSpawn(Vector2 t)
    {
        /// Check if a location is within the player Spawn Zone
        /// Args:
        ///     Vector2 t: The location to check
        /// Returns:
        ///     bool: True if the location is within the player Spawn Zone, false otherwise
        return (Utils.calculateDistance(t, playerSpawn) <= spawnRadius);
    }
    public bool checkIsEnemySpawn(Vector2 t)
    {
        /// Check if a location is within the enemy Spawn Zone
        /// Args:
        ///     Vector2 t: The location to check
        /// Returns:
        ///     bool: True if the location is within the enemy Spawn Zone, false otherwise
        return (Utils.calculateDistance(t, enemySpawn) <= spawnRadius);
    }
    public Tile GetEnemySpawnTile()
    {
        /// Get a random tile that an Enemy Unit can spawn on.
        /// This is a tile that:
        /// 1. Is in the Enemy's Spawn Zone.
        /// 2. Is currently able to be walked on.
        /// Returns
        ///     Tile: A random tile;
        ///     Null if no valid tile can be found
        try
        {
            return _tiles.Where(
                t => (checkIsEnemySpawn(t.Key))
                && t.Value.Walkable).
                OrderBy(t => UnityEngine.Random.value).First().Value;
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("No tile available");
            return null;
        }
    }

    public Tile getTile(Vector2 location)
    {
        /// Get the tile at a location
        /// Args:
        ///     Vector2 location: The location to get a tile for
        /// Returns:
        ///     The tile at the given location; null if no tile exists at that location
        if (_tiles.ContainsKey(location))
        {
            return _tiles[location];
        }
        else return null;
    }
    public int getGridSize()
    {
        /// Return the size of the grid
        /// Returns:
        ///     int _gridSize: The grid size.
        return _gridSize;
    }
}



public class GridDataManager
{
    string saveFile;
    public GridData data = new GridData();

    public GridDataManager(string filename)
    {
        saveFile = Application.dataPath + $"/Maps/{filename}.json";
    }

    public void saveGridData()
    {
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
    public SpawnLocation playerSpawn;
    public SpawnLocation enemySpawn;
    public BuildingData coreBuilding;
    public List<BuildingData> _buildings;

    public void storeSpawnRadius(int radius)
    {
        spawnRadius = radius;
    }
    public void storePlayerSpawn(Vector2 location)
    {
        playerSpawn = new SpawnLocation((int)location.x, (int)location.y);
    }
    public void storeEnemySpawn(Vector2 location)
    {
        enemySpawn = new SpawnLocation((int)location.x, (int)location.y);
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
}


[System.Serializable]
public class BuildingData
{
    public string buildingName;
    public float origin_x;
    public float origin_y;
    public int buildingKey;

}

