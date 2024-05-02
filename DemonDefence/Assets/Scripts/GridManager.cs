using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// All functionality relating to the set up of the grid
    /// </summary>
    [SerializeField] private int _gridSize;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _buildingTilePrefab;
    [SerializeField] private Dictionary<Vector2, Tile> _tiles;
    public CameraController cameraObject;
    [SerializeField] private BuildingRegister register;

    void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;

    }

    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        if (state == GameState.CreateGrid)
        {
            ///Create the grid and set the camera stats
            GenerateGrid();
            cameraObject.Init(_gridSize, 10);
        }
    }

    void GenerateGrid()
    {
        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector2 location = new Vector2(x, z);
                if (_tiles.ContainsKey(location)){
                    continue;
                }
                var placeBuilding = Random.Range(0, 5) == 3;
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
    }

    void placeTile(Tile tileToPlace, Vector2 location)
    {
        /// Place a tile of type 'tileToPlace' at location 'location'
        var spawnedTile = Instantiate(tileToPlace, vector2to3(location) * 10, Quaternion.identity);
        spawnedTile.name = $"Tile {location.x} {location.y}";

        spawnedTile.Init(vector2to3(location));
        _tiles[location] = spawnedTile;
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

}
