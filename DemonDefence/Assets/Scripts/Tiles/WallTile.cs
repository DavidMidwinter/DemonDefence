using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : BuildingTile
{
    /// <summary>
    /// Functionality for building tiles. Building tiles contain structures and are not able to be moved to or on by any units (currently)
    /// </summary>
    new public void Awake()
    {
        GridManager.UpdateTiles += setTile;
        base.Awake();
    }

    private bool checkTileIsWall(string tileName)
    {
        return tileName.Equals("WallTile") || tileName.Equals("GateTile");
    }
    public void setTile()
    {
        GameObject prefab;
        Quaternion facing = Quaternion.identity;
        string upTile = neighbours.Find(u => u.get2dLocation().y == get2dLocation().y + 10).GetType().ToString();
        string downTile = neighbours.Find(u => u.get2dLocation().y == get2dLocation().y - 10).GetType().ToString();
        string leftTile = neighbours.Find(u => u.get2dLocation().x == get2dLocation().x + 10).GetType().ToString();
        string rightTile = neighbours.Find(u => u.get2dLocation().x == get2dLocation().x - 10).GetType().ToString();

        bool up = checkTileIsWall(upTile);
        bool down = checkTileIsWall(downTile);
        bool left = checkTileIsWall(leftTile);
        bool right = checkTileIsWall(rightTile);
        switch (up, down, left, right) {
            case (true, true, false, false): //Up-Down
                prefab = GridManager.Instance.register.get_straight_wall();
                facing = Quaternion.Euler(0, 90, 0);
                break;
            case (false, false, true, true): //Left-Right
                prefab = GridManager.Instance.register.get_straight_wall();
                break;
            case (true, false, true, false): //Up-Left
                prefab = GridManager.Instance.register.get_corner_wall();
                break;
            case (true, false, false, true): //Up-Right
                prefab = GridManager.Instance.register.get_corner_wall();
                facing = Quaternion.Euler(0, 270, 0);
                break;
            case (false, true, true, false): //Down-Left
                prefab = GridManager.Instance.register.get_corner_wall();
                facing = Quaternion.Euler(0, 90, 0);
                break;
            case (false, true, false, true): //Down-Right
                prefab = GridManager.Instance.register.get_corner_wall();
                facing = Quaternion.Euler(0, 180, 0);
                break;
            default:
                prefab = new GameObject();
                break;
        }
        
        GameObject wall = Instantiate(prefab, transform.position, facing);
        wall.transform.parent = gameObject.transform;
    }

}
