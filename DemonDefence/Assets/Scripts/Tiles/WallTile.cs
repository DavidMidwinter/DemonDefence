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


        Debug.Log($"{name} {up} {down} {left} {right}");
        if(up && down)
        {
            prefab = GridManager.Instance.register.get_straight_wall();
            facing = Quaternion.Euler(0, 90, 0);
        }
        else if (left && right)
        {
            prefab = GridManager.Instance.register.get_straight_wall();
        }
        else if (left && up)
        {
            prefab = GridManager.Instance.register.get_corner_wall();
        }
        else if (right && up)
        {
            prefab = GridManager.Instance.register.get_corner_wall();
            facing = Quaternion.Euler(0, 270, 0);
        }
        else if (left && down)
        {
            prefab = GridManager.Instance.register.get_corner_wall();
            facing = Quaternion.Euler(0, 90, 0);
        }
        else if (right && down)
        {
            prefab = GridManager.Instance.register.get_corner_wall();
            facing = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            prefab = new GameObject();
        }
        Debug.Log(facing);
        GameObject wall = Instantiate(prefab, transform.position, facing);
        wall.transform.parent = gameObject.transform;
    }

}
