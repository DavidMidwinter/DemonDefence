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
    private int _numberOfPlayerSpawns = 1, _numberOfEnemySpawns = 1;
    [SerializeField] private string coreType;
    private int _maxBuildings = -1;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _buildingTilePrefab;
    [SerializeField] private Tile _grassTilePrefab;
    [SerializeField] private TreeTile _treeTilePrefab;
    [SerializeField] private BushTile _bushTilePrefab;
    [SerializeField] private Tile _wallTilePrefab;
    [SerializeField] private Tile _wallGatePrefab;
    [SerializeField] private Tile _waterTilePrefab;
    [SerializeField] private Tile _bridgeTilePrefab;
    private Dictionary<Vector2, Tile> _tiles;
    private string fileName;
    private GridDataManager gridDataManager;
    public CameraController cameraObject;
    public Building coreBuilding;
    [SerializeField] public BuildingRegister register;
    public static GridManager Instance;
    private Vector2[] directNeighbours = {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };
    private Vector2[] diagonalNeighbours =
    {
        new Vector2(1, 1),
        new Vector2(1, -1),
        new Vector2(-1, -1),
        new Vector2(-1, 1)
    };
    private Vector2[] validNeighbours;
    private int spawnRadius;
    private List<Spawnpoint> enemySpawns;
    private List<Spawnpoint> playerSpawns;
    public delegate void notifyTiles();
    public static event notifyTiles UpdateTiles;
    [SerializeField] private int treeChance = 25;
    [SerializeField] private int bushChance = 25;
    [SerializeField] private int rivers = 3;
    public Vector2 centrepoint;
    public Light worldLight;
    [SerializeField] Material daySkybox, nightSkybox, grassMaterial, stoneMaterial;
    [SerializeField] private GameObject groundBase;

    void Awake()
    {
        Instance = this;
        Debug.Log(Application.dataPath);
        List<Vector2> neighbours = new List<Vector2>();
        neighbours.AddRange(directNeighbours);
        if (GameManager.Instance.allowDiagonalMovement)
            neighbours.AddRange(diagonalNeighbours);

        validNeighbours = neighbours.ToArray();
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
    public void setNumberOfSpawns(int player, int enemy)
    {
        _numberOfEnemySpawns = enemy;
        _numberOfPlayerSpawns = player;
    }

    public void setFoliageChances(int trees, int bushes)
    {
        treeChance = trees;
        bushChance = bushes;
    }
    public void setRivers(int water)
    {
        rivers = water;
    }

    public void GenerateGrid()
    {

        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();
        gridDataManager = new GridDataManager(fileName);
        if (fileName != "")
        {
            if (Directory.Exists(gridDataManager.dataStore))
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

    void setEnemySpawnpoints()
    {
        List<UnitType> enemyKeywords = new List<UnitType>{ UnitType.Cultist, UnitType.Demonic, UnitType.Despoiler };
        enemySpawns = new List<Spawnpoint>();
        for(int i = 0; i < _numberOfEnemySpawns; i++)
        {
            Vector2 point = _tiles.Where(t => checkSpawnable(t.Value)
            && !(enemySpawns.Exists(spawn => spawn.location == t.Key))
            && (t.Key.x > _gridSize / 2 || t.Key.y > _gridSize / 2)
            && !inRangeOfEnemySpawns(t.Key)
            && !inRangeOfPlayerSpawns(t.Key)
            ).OrderBy(t => UnityEngine.Random.value).First().Key;

            enemySpawns.Add(new Spawnpoint(point, enemyKeywords));
        }
    }
    void setPlayerSpawnpoints()
    {
        List<UnitType> playerKeywords = new List<UnitType> { UnitType.Common, UnitType.Pious, UnitType.Mechanised };
        playerSpawns = new List<Spawnpoint>();
        for (int i = 0; i < _numberOfPlayerSpawns; i++)
        {
            Vector2 point = _tiles.Where(t => checkSpawnable(t.Value)
            && !(playerSpawns.Exists(spawn => spawn.location == t.Key))
            && (t.Key.x < _gridSize / 2 && t.Key.y < _gridSize / 2)
            && !inRangeOfEnemySpawns(t.Key)
            && !inRangeOfPlayerSpawns(t.Key)
            ).OrderBy(t => UnityEngine.Random.value).First().Key;

            playerSpawns.Add(new Spawnpoint(point, playerKeywords));
        }
    }
    bool inRangeOfEnemySpawns(Vector2 t)
    {
        return inRangeOfSpawns(t, enemySpawns);
    }
    bool inRangeOfPlayerSpawns(Vector2 t)
    {
        return inRangeOfSpawns(t, playerSpawns);
    }

    bool inRangeOfSpawns(Vector2 t, List<Spawnpoint> spawns)
    {
        if (spawns is null || spawns.Count == 0) return false;
        foreach (Spawnpoint e in spawns)
        {
            if (Utils.calculateDistance(t, e.location) < spawnRadius) return true;
        }
        return false;
    }
    void loadExistingGrid()
    {
        /// Load an existing grid from a JSon file
        Debug.Log($"Load grid {fileName}");
        gridDataManager.loadGridData();
        _gridSize = gridDataManager.getGridSize();
        playerSpawns = gridDataManager.getPlayerSpawns();
        enemySpawns = gridDataManager.getEnemySpawns();

        spawnRadius = gridDataManager.getSpawnRadius();
        _citySize = gridDataManager.getCitySize();
        setMapCentre();


        if (gridDataManager.getCoreBuilding() != null && gridDataManager.getCoreBuilding().buildingName != "")
        {
            Vector2 location = new Vector2(gridDataManager.getCoreBuilding().origin_x, gridDataManager.getCoreBuilding().origin_y);

            Building buildingToPlace = Instantiate(register.getCoreBuilding(gridDataManager.getCoreBuilding().buildingName),
                        vector2to3(location) * 10, Quaternion.identity);

            buildingToPlace.setTiles(location);

            buildingToPlace.name = $"{gridDataManager.getCoreBuilding().buildingName} {location.x} {location.y}";

            placeBuilding(buildingToPlace);
        }
        foreach (GroundTileData groundTile in gridDataManager.GetGroundTileDatas())
        {
            switch (groundTile.variant)
            {
                case (groundTileType.grassTile):
                    placeTile(_grassTilePrefab, groundTile.location);
                    break;
                case (groundTileType.stoneTile):
                    placeTile(_tilePrefab, groundTile.location);
                    break;
                case (groundTileType.waterTile):
                    placeTile(_waterTilePrefab, groundTile.location);
                    break;
                default:
                    placeTile(_tilePrefab, groundTile.location);
                    break;
            }

        }


        foreach (BuildingData building in gridDataManager.getBuildings())
        {
            Vector2 location = new Vector2(building.origin_x, building.origin_y);

            Building buildingToPlace = Instantiate(register.get_specific_building_by_key(building.buildingName),
                        vector2to3(location) * 10, Quaternion.identity);

            buildingToPlace.setTiles(location);

            buildingToPlace.name = $"Building {location.x} {location.y}";

            placeBuilding(buildingToPlace, true);

        }

        foreach (Vector2 location in gridDataManager.getGates())
        {
            placeTile(_wallGatePrefab, location);
        }
        foreach (Vector2 location in gridDataManager.getWalls())
        {
            placeTile(_wallTilePrefab, location);
        }
        foreach (Vector2 location in gridDataManager.getBridges())
        {
            placeTile(_bridgeTilePrefab, location);
        }

        foreach (FoliageData foliage in gridDataManager.getFoliage())
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

        UpdateTiles?.Invoke();
    }

    void generateRandomGrid(bool saveToFile)
    {
        /// Generate a new grid
        Debug.Log("Create new grid");
        int existingBuildings = 0;
        int centre = _gridSize / 2;
        List<Vector2> riverTiles = RiverGenerator.generateRivers(_gridSize, rivers);
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
                gridDataManager.storeCoreBuilding(coreBuildingData);
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
                        placeStoneTile(location);
                        Destroy(buildingToPlace.gameObject);
                    }
                }
                else if (riverTiles.Contains(location)
                    && (!_isCity || Utils.calculateDistance(location, centrepoint) > _citySize))
                {
                    placeWaterTile(location);
                }
                else
                {
                    placeGroundTile(location);
                }

            }
        }
        setEnemySpawnpoints();
        setPlayerSpawnpoints();

        if (saveToFile)
        {
            gridDataManager.storeSpawnRadius(spawnRadius);
            gridDataManager.storeBuildings(buildings);
            gridDataManager.storeGridSize(_gridSize);
            gridDataManager.storeCitySize(_citySize);
            gridDataManager.storeIsWalled(walled);
            gridDataManager.storeEnemySpawns(enemySpawns);
            gridDataManager.storePlayerSpawns(playerSpawns);
            gridDataManager.saveGridData();
        }

        UpdateTiles?.Invoke();
    }

    void placeGroundTile(Vector2 location, bool placeTrees = true)
    {
        float dist = Utils.calculateDistance(location, centrepoint);
        if (_isCity && dist <= _citySize)
        {
            placeStoneTile(location);
        }
        else
        {
            if (placeTrees)
            {
                int result = UnityEngine.Random.Range(0, 100);
                if (result < treeChance)
                {
                    placeTreeTile(location);
                }
                else if (result < treeChance + bushChance)
                {
                    placeBushTile(location);
                }
                else
                {
                    placeGrassTile(location);
                }
            }
            else
            {
                placeGrassTile(location);
            }
        }
    }

    void placeWaterTile(Vector2 location)
    {
        placeTile(_waterTilePrefab, location);
        gridDataManager.storeGroundTile(location, groundTileType.waterTile);
    }
    void placeStoneTile(Vector2 location)
    {
        placeTile(_tilePrefab, location);
        gridDataManager.storeGroundTile(location, groundTileType.stoneTile);
    }
    void placeGrassTile(Vector2 location)
    {
        placeTile(_grassTilePrefab, location);
        gridDataManager.storeGroundTile(location, groundTileType.grassTile);
    }
    void placeTreeTile(Vector2 location)
    {
        placeTile(_treeTilePrefab, location);
        storeFoliageData(location);
    }
    void placeBushTile(Vector2 location)
    {
        placeTile(_bushTilePrefab, location);
        storeFoliageData(location);
    }

    void buildWall(Vector2 centrepoint)
    {
        if (walled)
        {
            placeGate(new Vector2(centrepoint.x - _citySize, centrepoint.y));
            placeGate(new Vector2(centrepoint.x + _citySize, centrepoint.y));
            placeGate(new Vector2(centrepoint.x, centrepoint.y - _citySize));
            placeGate(new Vector2(centrepoint.x, centrepoint.y + _citySize));


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
                        placeWall(location);
                        placeWall(new Vector2(centrepoint.x + x, centrepoint.y - y));
                        placeWall(new Vector2(centrepoint.x - x, centrepoint.y + y));
                        placeWall(new Vector2(centrepoint.x + x, centrepoint.y + y));
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
                        placeWall(location);
                        placeWall(new Vector2(centrepoint.x + x, centrepoint.y - y));
                        placeWall(new Vector2(centrepoint.x - x, centrepoint.y + y));
                        placeWall(new Vector2(centrepoint.x + x, centrepoint.y + y));
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

    void placeWall(Vector2 location)
    {
        placeTile(_wallTilePrefab, location);
        gridDataManager.storeWall(location);
    }

    void placeGate(Vector2 location)
    {
        placeTile(_wallGatePrefab, location);
        gridDataManager.storeGate(location);
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
                gridDataManager.storeFoliage(tile.foliageInfo());
            }
            else if (_tiles[location].GetType() == typeof(BushTile))
            {
                BushTile tile = (BushTile)_tiles[location];
                gridDataManager.storeFoliage(tile.foliageInfo());
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

            if(Utils.calculateDistance(t, centrePoint) > _citySize) return false;

        }
        return true;
    }

    void placeBuilding(Building buildingToPlace, bool borderless = false)
    {
        /// Place a Building on the grid.
        /// Args:
        ///     Building buildingToPlace: The building to place on the grid.
        foreach (Vector2 t in buildingToPlace.getTiles())
        {
            placeTile(_buildingTilePrefab, t);

        }

        if (borderless) return; // Ends placement if borders are ignored

        foreach (Vector2 t in buildingToPlace.getBorderTiles())
        {
            if (t.x < _gridSize && t.y < _gridSize)
            {
                placeStoneTile(t);
            }
        }
    }

    public bool checkSpawnable(Tile t)
    {
        return t.Walkable && !t.isWater;
    }
    public Tile GetPlayerSpawnTile(List<UnitType> spawnTypes)
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
                return getSpawnTileFromList(spawnTypes, playerSpawns);
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("No tile available");
            return null;
        }
    }

    public Tile getSpawnTileFromList(List<UnitType> spawnTypes, List<Spawnpoint> spawnList)
    {

        Spawnpoint spawnToUse = spawnList.Where(e => e.validUnits.Count == 0 || e.validUnits.Exists(uT => spawnTypes.Contains(uT))).OrderBy(e => e.numberOfSpawns).ThenBy(t => UnityEngine.Random.value).First();
        Debug.Log($"Spawn at {spawnToUse}, {spawnToUse.numberOfSpawns} have spawned here");
        spawnToUse.numberOfSpawns++;
        return _tiles.Where(
                    t => (Utils.calculateDistance(t.Key, spawnToUse.location) <= spawnRadius)
                    && checkSpawnable(t.Value)
                    ).OrderBy(t => UnityEngine.Random.value).First().Value;
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
    public Tile GetNearestSpawnableTile(Tile origin, int minimumDistance = 0)
    {
        try
        {
            return _tiles.Where(
                t => checkSpawnable(t.Value) &&
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
    public Tile GetEnemySpawnTile(List<UnitType> spawnTypes)
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
            return getSpawnTileFromList(spawnTypes, enemySpawns);
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
        if (GameManager.Instance.isNight) setNightLighting();
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
    public string dataStore;
    public string saveDirectory = Path.Combine(Application.dataPath, "Maps");
    public GridData gridData = new GridData();
    public SpawnData spawnData = new SpawnData();

    public GridDataManager(string filename)
    {

        dataStore = Path.Combine(saveDirectory, $"{filename}");
    }

    public void saveGridData()
    {
        Directory.CreateDirectory(saveDirectory);
        Directory.CreateDirectory(dataStore);

        string gridDataString = JsonUtility.ToJson(gridData, true);
        File.WriteAllText(tileMapLocation(), gridDataString);

        string spawnDataString = JsonUtility.ToJson(spawnData, true);
        File.WriteAllText(spawnMapLocation(), spawnDataString);
    }
    public void loadGridData()
    {
        gridData = JsonUtility.FromJson<GridData>(File.ReadAllText(tileMapLocation()));
        spawnData = JsonUtility.FromJson<SpawnData>(File.ReadAllText(spawnMapLocation()));
    }

    private string tileMapLocation()
    {
        return Path.Combine(dataStore, "tilemap.json");
    }
    private string spawnMapLocation()
    {
        return Path.Combine(dataStore, "spawnmap.json");
    }

    public void cleanData()
    {
        gridData.cleanData();
        spawnData.cleanData();
    }
    public void storeSpawnRadius(int value)
    {
        spawnData.spawnRadius = value;
    }
    public void storeIsWalled(bool isWalled)
    {
        gridData.isWalled = isWalled;
    }
    
    public void storeEnemySpawns(List<Spawnpoint> spawnLocations)
    {
        spawnData.storeEnemySpawns(spawnLocations);
    }

    public void storePlayerSpawns(List<Spawnpoint> spawnLocations)
    {
        spawnData.storePlayerSpawns(spawnLocations);
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

    public BuildingData getCoreBuilding()
    {
        return gridData.coreBuilding;
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
        return spawnData.spawnRadius;
    }

    public List<Spawnpoint> getPlayerSpawns()
    {
        return spawnData.playerSpawnLocations;
    }

    public List<Spawnpoint> getEnemySpawns()
    {
        return spawnData.enemySpawnLocations;
    }
}

[System.Serializable]
public class SpawnData
{
    public int spawnRadius;
    public List<Spawnpoint> playerSpawnLocations;
    public List<Spawnpoint> enemySpawnLocations;

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
public class Spawnpoint
{
    public Vector2 location;
    public List<UnitType> validUnits;

    [NonSerialized]
    public int numberOfSpawns = 0;

    public Spawnpoint(Vector2 spawnLocation, List<UnitType> unitTypes = null)
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

public enum groundTileType
{
    stoneTile,
    grassTile,
    waterTile
}

