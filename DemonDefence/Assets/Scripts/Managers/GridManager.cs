using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// All functionality relating to the set up of the grid
    /// </summary>
    /// 


    [SerializeField] private int _gridSize;
    [SerializeField] private int _maxBuildings = -1;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _buildingTilePrefab;
    [SerializeField] private Dictionary<Vector2, Tile> _tiles;
    public CameraController cameraObject;
    [SerializeField] private BuildingRegister register;
    public static GridManager Instance;
    private Vector2[] validNeighbours = { 
        new Vector2(0, 1), 
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };
    void Awake()
    {
        Instance = this;
    }


    public void GenerateGrid()
    {
        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();
        int existingBuildings = 0;
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector2 location = new Vector2(x, z);
                if (_tiles.ContainsKey(location)) {
                    continue;
                }
                var placeBuilding = false;

                if (_maxBuildings == -1 || existingBuildings < _maxBuildings)
                {
                    placeBuilding = Random.Range(0, 5) == 3;
                }
                

                if (placeBuilding)
                {
                    Building buildingToPlace = Instantiate(register.get_random_building(), 
                        vector2to3(location) * 10, Quaternion.identity);
                    
                    buildingToPlace.setTiles(location);
                    buildingToPlace.name = $"Building {location.x} {location.y}";
                    if (evaluateBuildingPlacement(buildingToPlace))
                    {
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
                        existingBuildings += 1;
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
        cameraObject.Init(_gridSize, 10);
        GameManager.Instance.UpdateGameState(GameState.SpawnPlayer);
    }

    void placeTile(Tile tileToPlace, Vector2 location)
    {
        /// Place a tile of type 'tileToPlace' at location 'location'
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
                Debug.Log($"{location} is a neighbour of {neighbourLocation}");
            }
        }
    }


    Vector3 vector2to3(Vector2 vector)
    {
        /// Convert a vector 2 to a vector 3. This specifically sets the new vector3 to 'x, 0, y'
        return new Vector3(vector.x, 0, vector.y);
    }

    bool evaluateBuildingPlacement(Building buildingToEvaluate)
    {   
        foreach (Vector2 t in buildingToEvaluate.getAllTiles())
        {
            if (t.x >= _gridSize || t.y >= _gridSize)
            {
                Debug.Log(t);
                return false;
            }
            
            if (_tiles.ContainsKey(t))
            {
                return false;
            }

        }
        return true;
    }

    public Tile GetPlayerSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < _gridSize / 2 && t.Value.Walkable).
            OrderBy(t => Random.value).First().Value;
    }
    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _gridSize / 2 && t.Value.Walkable).
            OrderBy(t => Random.value).First().Value;
    }

    public Tile getTile(Vector2 location)
    {
        if (_tiles.ContainsKey(location))
        {
            return _tiles[location];
        }
        else return null;
    }
    public int getGridSize()
    {
        return _gridSize;
    }
}
