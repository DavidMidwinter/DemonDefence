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
    [SerializeField] private Dictionary<Vector2, Tile> _tiles;
    public CameraController camera;
    private void Start()
    {   
        ///Create the grid and set the camera stats
        GenerateGrid();
        camera.Init(_gridSize, 10);

    }

    void GenerateGrid()
    {
        /// Generate a grid of tile objects to the size specified in _gridSize.
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector3 location = new Vector3(10 * x, 0, 10 * z);
                var spawnedTile = Instantiate(_tilePrefab, location, Quaternion.identity);
                spawnedTile.name = $"Tile {x} {z}";

                var isOffset = (x % 2 == 0 && z % 2 != 0) || (z % 2 == 0 && x % 2 != 0);
                spawnedTile.Init(isOffset);
                _tiles[new Vector2(x, z)] = spawnedTile;
            }
        }
    }
}
