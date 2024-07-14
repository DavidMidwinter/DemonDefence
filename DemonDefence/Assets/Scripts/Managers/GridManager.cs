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


    private int _gridSize;
    private bool _isCity;
    private int _citySize = 0;
    private bool walled;
    [SerializeField] private string coreType;
    private int _maxBuildings = -1;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _buildingTilePrefab;
    [SerializeField] private Tile _grassTilePrefab;
    [SerializeField] private TreeTile _treeTilePrefab;
    [SerializeField] private BushTile _bushTilePrefab;
    [SerializeField] private Tile _wallTilePrefab;
    [SerializeField] private Tile _wallGatePrefab;
    private Dictionary<Vector2, Tile> _tiles;
    private string fileName;
    private GridDataManager gridDataManager;
    public CameraController cameraObject;
    public Building coreBuilding;
    [SerializeField] public BuildingRegister register;
    public static GridManager Instance;
    private Vector2[] validNeighbours = {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };
    private int spawnRadius;
    private Vector2 playerSpawn;
    private Vector2 enemySpawn;
    public delegate void notifyTiles();
    public static event notifyTiles UpdateTiles;
    [SerializeField] private int treeChance = 25;
    [SerializeField] private int bushChance = 25;
    public Vector2 centrepoint;
    public Light worldLight;
    [SerializeField] private bool isNight;
    [SerializeField] Material daySkybox, nightSkybox, grassMaterial, stoneMaterial;
    [SerializeField] private GameObject groundBase;

    void Awake()
    {
        Instance = this;
        Debug.Log(Application.dataPath);
    }

    

    public void setGridSize(int size)
    {
        _gridSize = size;
    }

    public void setIsCity(bool toggle)
    {
        _isCity = toggle;
    }
    public void setCitySize(int size)
    {
        _citySize = size;
    }

    public void setMaxBuildings(int buildingNumber)
    {
        _maxBuildings = buildingNumber;
    }

    public void setFileName(string name)
    {
        fileName = name;
    }

    public void setSpawnRadius(int radius)
    {
        spawnRadius = radius;
    }
    public void setWalled(bool toggle)
    {
        walled = toggle;
    }

    public void setFoliageChances(int trees, int bushes)
    {
        treeChance = trees;
        bushChance = bushes;
    }

    public void setIsNight(bool toggle)
    {
        isNight = toggle;
    }

    public void GenerateGrid()
    {

        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();
        gridDataManager = new GridDataManager(fileName);
        if (fileName != "")
        {
            if (File.Exists(gridDataManager.saveFile))
            {
                loadExistingGrid();
            }
            else
            {
                generateRandomGrid(true);
            }
        }
        else
            generateRandomGrid(false);


        setEnvironmentLighting();
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
        _citySize = gridDataManager.data.citySize;
        setMapCentre();


        if (gridDataManager.data.coreBuilding != null)
        {
            Vector2 location = new Vector2(gridDataManager.data.coreBuilding.origin_x, gridDataManager.data.coreBuilding.origin_y);

            Building buildingToPlace = Instantiate(register.getCoreBuilding(gridDataManager.data.coreBuilding.buildingName),
                        vector2to3(location) * 10, Quaternion.identity);

            buildingToPlace.setTiles(location);

            buildingToPlace.name = $"{gridDataManager.data.coreBuilding.buildingName} {location.x} {location.y}";

            placeBuilding(buildingToPlace);
        }

        if (gridDataManager.data.isWalled)
            buildWall(centrepoint);

        foreach (BuildingData building in gridDataManager.data._buildings)
        {
            Vector2 location = new Vector2(building.origin_x, building.origin_y);

            Building buildingToPlace = Instantiate(register.get_specific_building_by_key(building.buildingName),
                        vector2to3(location) * 10, Quaternion.identity);

            buildingToPlace.setTiles(location);

            buildingToPlace.name = $"Building {location.x} {location.y}";

            placeBuilding(buildingToPlace);

        }

        foreach (FoliageData foliage in gridDataManager.data._foliage)
        {
            Vector2 location = new Vector2(foliage.x, foliage.y);
            switch (foliage.type){
                case 0:
                    placeTile(_treeTilePrefab, location);
                    TreeTile treetile = (TreeTile)_tiles[location];
                    treetile.setRotation(foliage.rotationY, foliage.rotationW);
                    treetile.setScale(foliage.scale);
                    break;

                case 1:
                    placeTile(_bushTilePrefab, location);
                    BushTile bushtile = (BushTile)_tiles[location];
                    bushtile.setRotation(foliage.rotationY, foliage.rotationW);
                    bushtile.setScale(foliage.scale);
                    break;

                default:
                    continue;
            }
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
                placeGroundTile(location, false);
            }
        }

        UpdateTiles?.Invoke();
    }

    void generateRandomGrid(bool saveToFile)
    {
        /// Generate a new grid
        Debug.Log("Create new grid");
        playerSpawn = new Vector2(spawnRadius, spawnRadius);
        enemySpawn = new Vector2(_gridSize - spawnRadius, _gridSize - spawnRadius);
        int existingBuildings = 0;
        int centre = _gridSize / 2;
        setMapCentre();
        if (!_isCity)
            _citySize = 0;
        else if (_citySize < _gridSize / 4) 
            _citySize = _gridSize / 4;
        else if (_citySize >= _gridSize / 2)
        {
            _citySize = _gridSize;
            walled = false;
        }
        List<BuildingData> buildings = new List<BuildingData>();

        if (_isCity)
        {
            Building coreTemplate = register.getCoreBuilding(coreType);
            if (coreTemplate)
            {
                Vector2 core_location = new Vector2(centre - 1, centre - 1);
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

            buildWall(centrepoint);
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
                if (_isCity
                    && (_maxBuildings == -1 || existingBuildings < _maxBuildings)
                    && Utils.calculateDistance(location, centrepoint) <= _citySize
                    && UnityEngine.Random.Range(0, 5) == 3)
                {
                    int buildingKey = register.get_random_building();

                    Building buildingToPlace = Instantiate(register.get_specific_building(buildingKey),
                        vector2to3(location) * 10, Quaternion.identity);

                    buildingToPlace.setTiles(location);

                    buildingToPlace.name = $"Building {location.x} {location.y}";

                    if (evaluateBuildingPlacement(buildingToPlace, centrepoint))
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
                    placeGroundTile(location);
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
            gridDataManager.data.storeCitySize(_citySize);
            gridDataManager.data.isWalled = walled;
            gridDataManager.saveGridData();
        }

        UpdateTiles?.Invoke();
    }

    void placeGroundTile(Vector2 location, bool placeTrees = true)
    {
        float dist = Utils.calculateDistance(location, centrepoint);
        if (_isCity && dist <= _citySize)
            placeTile(_tilePrefab, location);
        else
        {
            if (placeTrees)
            {
                if (checkIsPlayerSpawn(location) || checkIsEnemySpawn(location))
                {
                    placeTile(_grassTilePrefab, location);
                    return;
                }

                int result = UnityEngine.Random.Range(0, 100);
                if (result < treeChance)
                {
                    placeTile(_treeTilePrefab, location);
                    storeFoliageData(location);
                }
                else if (result < treeChance + bushChance)
                {
                    placeTile(_bushTilePrefab, location);
                    storeFoliageData(location);
                }
                else
                {
                    placeTile(_grassTilePrefab, location);
                }
            }
            else
                placeTile(_grassTilePrefab, location);
        }
    }
    void buildWall(Vector2 centrepoint)
    {
        if (walled)
        {
            placeTile(_wallGatePrefab, new Vector2(centrepoint.x - _citySize, centrepoint.y));
            placeTile(_wallGatePrefab, new Vector2(centrepoint.x + _citySize, centrepoint.y));
            placeTile(_wallGatePrefab, new Vector2(centrepoint.x, centrepoint.y - _citySize));
            placeTile(_wallGatePrefab, new Vector2(centrepoint.x, centrepoint.y + _citySize));


            List<Vector2> firstPass = new List<Vector2>(); // First pass gets all locations that are on the circle line.
            for(int x = 1; x <= _citySize; x++)
            {
                for(int y = 1; y <= _citySize; y++)
                {
                    Vector2 location = new Vector2(centrepoint.x - x, centrepoint.y - y);
                    float dist = Utils.calculateDistance(location, centrepoint);

                    bool onCircle = (dist > _citySize && dist <= _citySize + 1);

                    if (onCircle)
                    {
                        firstPass.Add(location);
                        placeTile(_wallTilePrefab, location);
                        placeTile(_wallTilePrefab, new Vector2(centrepoint.x + x, centrepoint.y - y));
                        placeTile(_wallTilePrefab, new Vector2(centrepoint.x - x, centrepoint.y + y));
                        placeTile(_wallTilePrefab, new Vector2(centrepoint.x + x, centrepoint.y + y));
                    }
                }
            }
            for (int x = 0; x <= _citySize; x++)// Second pass gets all locations that are within the circle line and adjacent to two non-connected tiles.
            {
                for (int y = 0; y <= _citySize; y++)
                {
                    Vector2 location = new Vector2(centrepoint.x - x, centrepoint.y - y);
                    Vector2 xadj = new Vector2(centrepoint.x - (x + 1), centrepoint.y - y);
                    Vector2 yadj = new Vector2(centrepoint.x - x, centrepoint.y - (y + 1));
                    Vector2 xyadj = new Vector2(centrepoint.x - (x+1), centrepoint.y - (y + 1));
                    float dist = Utils.calculateDistance(location, centrepoint);

                    if (dist < _citySize+1
                        && firstPass.Contains(xadj) 
                        && firstPass.Contains(yadj) 
                        && !firstPass.Contains(location)
                        && !firstPass.Contains(xyadj)
                        )
                    {
                        placeTile(_wallTilePrefab, location);
                        placeTile(_wallTilePrefab, new Vector2(centrepoint.x + x, centrepoint.y - y));
                        placeTile(_wallTilePrefab, new Vector2(centrepoint.x - x, centrepoint.y + y));
                        placeTile(_wallTilePrefab, new Vector2(centrepoint.x + x, centrepoint.y + y));
                    }
                }
            }

            for(int x = 0; x <= _citySize+2; x++)
            {
                Vector2 location = new Vector2(centrepoint.x - x, centrepoint.y);
                float dist = Utils.calculateDistance(location, centrepoint);
                if (!_tiles.ContainsKey(location))
                {
                    placeGroundTile(location, false);
                    placeGroundTile(new Vector2(centrepoint.x + x, centrepoint.y), false);
                    placeGroundTile(new Vector2(centrepoint.x, centrepoint.y - x), false);
                    placeGroundTile(new Vector2(centrepoint.x, centrepoint.y + x), false);
                }
            }


        }
    }
    void placeTile(Tile tileToPlace, Vector2 location)
    {
        /// Place a tile of type 'tileToPlace' at location 'location'
        /// Args:
        ///     Tile tileToPlace: The tile to place
        ///     Vector2 location: The location to place the tile at - this is a vector 2, storing the 'x' and 'z' coords
        var spawnedTile = Instantiate(tileToPlace, vector2to3(location) * 10, Quaternion.identity);
        spawnedTile.name = $"{tileToPlace.GetType()} {location.x} {location.y}";

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

    public void storeFoliageData(Vector2 location)
    {
        if (_tiles[location])
        {
            if (_tiles[location].GetType() == typeof(TreeTile))
            {
                TreeTile tile = (TreeTile)_tiles[location];
                gridDataManager.data.storeFoliage(tile.foliageInfo());
            }
            else if (_tiles[location].GetType() == typeof(BushTile))
            {
                BushTile tile = (BushTile)_tiles[location];
                gridDataManager.data.storeFoliage(tile.foliageInfo());
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

    bool evaluateBuildingPlacement(Building buildingToEvaluate, Vector2 centrePoint)
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

            if(Utils.calculateDistance(t, centrePoint) > _citySize) return false;

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
                && !t.Value.getNeighbours().Any(
                    u => u.occupiedUnit != null 
                    && u.occupiedUnit.unitTypes.Contains(UnitType.Leader)
                    )
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

    public int getCitySize()
    {
        return _citySize;
    }
    public List<GateTile> getGates()
    {
        if (walled)
        {
            return _tiles.Values.Where(t => t is GateTile).Cast<GateTile>().ToList();
        }
        else return null;
    }

    private void setMapCentre() {
        centrepoint = new Vector2(_gridSize / 2, _gridSize / 2);
        worldLight.transform.position = new Vector3(centrepoint.x * 10, 70, centrepoint.y * 10);
        GameObject baseObject = Instantiate(
            groundBase, new Vector3(centrepoint.x * 10, -0.001f, centrepoint.y * 10), Quaternion.identity);
        baseObject.transform.localScale = Vector3.one * (_gridSize + 50);
        MeshRenderer baseRenderer = baseObject.GetComponent<MeshRenderer>();

        baseRenderer.material = (_isCity && _citySize >= _gridSize / 2) ? stoneMaterial : grassMaterial;


    }

    private void setEnvironmentLighting()
    {
        if (isNight) setNightLighting();
        else setDayLighting();
    }

    private void setNightLighting()
    {
        worldLight.color = new Color(0.663f, 0.859f, 1.000f, 0.5f);
        RenderSettings.skybox = nightSkybox;
    }

    private void setDayLighting()
    {
        worldLight.color = new Color(1.000f, 0.957f, 0.839f, 1f);
        RenderSettings.skybox = daySkybox;
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
    public BuildingData coreBuilding;
    public List<BuildingData> _buildings;
    public List<FoliageData> _foliage;

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

        if (_foliage == null) {
            _foliage = new List<FoliageData>();
        }
        _foliage.Add(newFoliage);
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

