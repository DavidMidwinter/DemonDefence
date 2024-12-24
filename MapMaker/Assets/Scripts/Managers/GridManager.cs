using System.Collections;
using System.Collections.Generic;
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

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        defaultTile = tileManager.getTile(tileType.grass);
        gridDataManager = new GridDataManager(fileName);
        generateEmptyGrid();
    }

    public void generateEmptyGrid() {
        _tiles = new Dictionary<Vector2, TileSlot>();
    
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                Debug.Log($"{x}, {y}");
                Vector2 coords = new Vector2 { x=x, y=y };
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

                newTile.setTileType(defaultTile);

            }
        }

    }

    public int getGridSize()
    {
        return _gridSize;
    }

    public void saveMap()
    {
        foreach(Vector2 location in _tiles.Keys)
        {
            if (_tiles[location].hasBuilding()) continue;
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
            gridDataManager.saveGridData();
        }
    }

    void storeFoliage(Vector2 location, TileSlot tile, int foliageType)
    {
        (float rotationY, float rotationW, float scale) foliageInfo = tile.getFoliageData();

        gridDataManager.data.storeFoliage((location, foliageInfo.rotationY, foliageInfo.rotationW, foliageInfo.scale, foliageType));
    }
}