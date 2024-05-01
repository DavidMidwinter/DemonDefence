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
    private void Start()
    {   
        ///Create the grid and set the camera stats
        GenerateGrid();
        cameraObject.Init(_gridSize, 10);

    }

    void GenerateGrid()
    {
        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector2 tile_key = new Vector2(x, z);
                if (_tiles.ContainsKey(tile_key)){
                    break;
                }
                var tileToPlace = _tilePrefab;

                Vector3 location = new Vector3(10 * x, 0, 10 * z);
                var placeBuilding = Random.Range(0, 5) == 3;
                if (placeBuilding)
                {
                    tileToPlace = _buildingTilePrefab;
                }
                var spawnedTile = Instantiate(tileToPlace, location, Quaternion.identity);
                spawnedTile.name = $"Tile {x} {z}";

                spawnedTile.Init(location);
                _tiles[tile_key] = spawnedTile;
            }
        }
    }

}
