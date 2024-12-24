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
    private Tile defaultTile;
    private List<Building> buildings;

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        _tiles = new Dictionary<Vector2, TileSlot>();
        defaultTile = tileManager.getTile(tileType.grass);
        if(fileName == "")
        {
            Debug.LogError("No filename specified");
            Application.Quit();
            return;
        }
        gridDataManager = new GridDataManager(fileName);

        if (File.Exists(gridDataManager.saveFile)) loadGrid();
        
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

    }

    public void loadGrid() {
        Debug.Log($"Load grid {fileName}");
        buildings = new List<Building>();
        gridDataManager.loadGridData();
        _gridSize = gridDataManager.data.gridSize;

        foreach(GroundTileData groundTile in gridDataManager.data._groundTiles)
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
        foreach(Vector2 location in gridDataManager.data._gateTiles)
        {
            placeTile(tileManager.getTile(tileType.gate), location);
        }
        foreach (Vector2 location in gridDataManager.data._wallTiles)
        {
            placeTile(tileManager.getTile(tileType.wall), location);
        }
        foreach (FoliageData foliage in gridDataManager.data._foliage)
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
        
        foreach(BuildingData buildingData in gridDataManager.data._buildings)
        {
            Vector2 origin = new Vector2(buildingData.origin_x, buildingData.origin_y);
            buildingType key = (buildingType)buildingData.buildingKey;
            _tiles[origin].placeBuilding(BuildingManager.Instance.getBuilding(key).prefab);
        }

        Debug.Log(_tiles.Count);


    }

    public int getGridSize()
    {
        return _gridSize;
    }

    public void saveMap()
    {
        gridDataManager.data.cleanData();
        foreach (Vector2 location in _tiles.Keys)
        {
            if (_tiles[location].hasBuilding()) continue;
            Debug.Log($"Save {location}");
            switch (_tiles[location].getType())
            {
                case tileType.grass:
                    gridDataManager.data.storeGroundTile(location, groundTileType.grassTile);
                    break;
                case tileType.stone:
                    gridDataManager.data.storeGroundTile(location, groundTileType.stoneTile);
                    break;
                case tileType.water:
                    gridDataManager.data.storeGroundTile(location, groundTileType.waterTile);
                    break;
                case tileType.wall:
                    gridDataManager.data.storeWall(location);
                    break;
                case tileType.gate:
                    gridDataManager.data.storeGate(location);
                    break;
                case tileType.tree:
                    storeFoliage(location, _tiles[location], 0);
                    break;
                case tileType.bush:
                    storeFoliage(location, _tiles[location], 1);
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
        gridDataManager.data.storeGridSize(_gridSize);
        gridDataManager.data.storeBuildings(buildingData);
        gridDataManager.saveGridData();
    }

    void storeFoliage(Vector2 location, TileSlot tile, int foliageType)
    {
        (float rotationY, float rotationW, float scale) foliageInfo = tile.getFoliageData();

        gridDataManager.data.storeFoliage((location, foliageInfo.rotationY, foliageInfo.rotationW, foliageInfo.scale, foliageType));

    }

    public void addBuilding(Building building)
    {
        buildings.Add(building);
    }
    public void removeBuilding(Building building)
    {
        buildings.Remove(building);
    }
    
}