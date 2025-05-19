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
    private void OnDestroy()
    {
        GridManager.UpdateTiles -= setTile;
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
            case (true, true, true, true): //Cross Junction
                prefab = GridManager.Instance.register.get_cross_junction_wall();
                Debug.LogWarning($"{this}[WallTile]: Cross Junction");
                break;
            case (true, true, true, false): //Up-Down-Left
                prefab = GridManager.Instance.register.get_junction_wall();
                Debug.LogWarning($"{this}[WallTile]: Up-Down-Left");
                break;
            case (false, true, true, true): //Down-Left-Right
                prefab = GridManager.Instance.register.get_junction_wall();
                Debug.LogWarning($"{this}[WallTile]: Down-Left-Right");
                facing = Quaternion.Euler(0, 90, 0);
                break;
            case (true, false, true, true): //Up-Left-Right
                prefab = GridManager.Instance.register.get_junction_wall();
                Debug.LogWarning($"{this}[WallTile]: Up-Left-Right");
                facing = Quaternion.Euler(0, 270, 0);
                break;
            case (true, true, false, true): //Up-Down-Right
                prefab = GridManager.Instance.register.get_junction_wall();
                Debug.LogWarning($"{this}[WallTile]: Up-Down-Right");
                facing = Quaternion.Euler(0, 180, 0);
                break;
            case (true, true, false, false): //Up-Down
                prefab = GridManager.Instance.register.get_straight_wall();
                facing = Quaternion.Euler(0, 90, 0);
                Debug.LogWarning($"{this}[WallTile]: Up-Down");
                break;
            case (false, false, true, true): //Left-Right
                prefab = GridManager.Instance.register.get_straight_wall();
                Debug.LogWarning($"{this}[WallTile]: Left-Right");
                break;
            case (true, false, true, false): //Up-Left
                prefab = GridManager.Instance.register.get_corner_wall();
                Debug.LogWarning($"{this}[WallTile]: Up-Left");
                break;
            case (true, false, false, true): //Up-Right
                prefab = GridManager.Instance.register.get_corner_wall();
                facing = Quaternion.Euler(0, 270, 0);
                Debug.LogWarning($"{this}[WallTile]: Up-Right");
                break;
            case (false, true, true, false): //Down-Left
                prefab = GridManager.Instance.register.get_corner_wall();
                facing = Quaternion.Euler(0, 90, 0);
                Debug.LogWarning($"{this}[WallTile]: Down-Left");
                break;
            case (false, true, false, true): //Down-Right
                prefab = GridManager.Instance.register.get_corner_wall();
                facing = Quaternion.Euler(0, 180, 0);
                Debug.LogWarning($"{this}[WallTile]: Down-Right");
                break;
            case (true, false, false, false): //Up
                prefab = GridManager.Instance.register.get_end_wall();
                facing = Quaternion.Euler(0, 270, 0);
                Debug.LogWarning($"{this}[WallTile]: Up");
                break;
            case (false, true, false, false): //Down
                prefab = GridManager.Instance.register.get_end_wall();
                facing = Quaternion.Euler(0, 90, 0);
                Debug.LogWarning($"{this}[WallTile]: Down");
                break;
            case (false, false, true, false): //Left
                prefab = GridManager.Instance.register.get_end_wall();
                Debug.LogWarning($"{this}[WallTile]: Left");
                break;
            case (false, false, false, true): //Right
                prefab = GridManager.Instance.register.get_end_wall();
                facing = Quaternion.Euler(0, 180, 0);
                Debug.LogWarning($"{this}[WallTile]: Right");
                break;
            default: //Individual tower
                prefab = GridManager.Instance.register.get_tower();
                Debug.LogWarning($"{this}[WallTile]: Tower");
                break;
        }
        
        GameObject wall = Instantiate(prefab, transform.position, facing);
        wall.transform.parent = gameObject.transform;
    }

}
