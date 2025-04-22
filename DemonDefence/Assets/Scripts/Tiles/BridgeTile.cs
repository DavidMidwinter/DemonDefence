using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTile : Ground
{
    new public void Awake()
    {
        GridManager.UpdateTiles += setTile;
        base.Awake();
    }
    new private void OnDestroy()
    {
        GridManager.UpdateTiles -= setTile;
        base.OnDestroy();
    }

    private bool checkTileIsConnectable(Tile tile)
    {
        return (tile.getBaseWalkable() && !tile.isWater);
    }
    public void setTile()
    {
        GameObject prefab;
        Quaternion facing = Quaternion.identity;
        Tile upTile = neighbours.Find(u => u.get2dLocation().y == get2dLocation().y + 10);
        Tile downTile = neighbours.Find(u => u.get2dLocation().y == get2dLocation().y - 10);
        Tile leftTile = neighbours.Find(u => u.get2dLocation().x == get2dLocation().x + 10);
        Tile rightTile = neighbours.Find(u => u.get2dLocation().x == get2dLocation().x - 10);

        bool up = checkTileIsConnectable(upTile);
        bool down = checkTileIsConnectable(downTile);
        bool left = checkTileIsConnectable(leftTile);
        bool right = checkTileIsConnectable(rightTile);
        switch (up, down, left, right)
        {
            case (true, true, true, true): //Cross Junction
                prefab = GridManager.Instance.register.get_bridge_cross();
                Debug.LogWarning($"{this}[BridgeTile]: Cross Junction");
                break;
            case (true, true, true, false): //Up-Down-Left
                prefab = GridManager.Instance.register.get_bridge_junction();
                Debug.LogWarning($"{this}[BridgeTile]: Up-Down-Left");
                break;
            case (false, true, true, true): //Down-Left-Right
                prefab = GridManager.Instance.register.get_bridge_junction();
                Debug.LogWarning($"{this}[BridgeTile]: Down-Left-Right");
                facing = Quaternion.Euler(0, 90, 0);
                break;
            case (true, false, true, true): //Up-Left-Right
                prefab = GridManager.Instance.register.get_bridge_junction();
                Debug.LogWarning($"{this}[BridgeTile]: Up-Left-Right");
                facing = Quaternion.Euler(0, 270, 0);
                break;
            case (true, true, false, true): //Up-Down-Right
                prefab = GridManager.Instance.register.get_bridge_junction();
                Debug.LogWarning($"{this}[BridgeTile]: Up-Down-Right");
                facing = Quaternion.Euler(0, 180, 0);
                break;
            case (true, true, false, false): //Up-Down
                Debug.LogWarning($"{this}[BridgeTile]: Up-Down-Right");
                prefab = GridManager.Instance.register.get_bridge_line();
                facing = Quaternion.Euler(0, 90, 0);
                break;
            case (false, false, true, true): //Left-Right
                Debug.LogWarning($"{this}[BridgeTile]: Up-Down-Right");
                prefab = GridManager.Instance.register.get_bridge_line();
                break;
            case (true, false, true, false): //Up-Left
                Debug.LogWarning($"{this}[BridgeTile]: Up-Left");
                prefab = GridManager.Instance.register.get_bridge_corner();
                break;
            case (true, false, false, true): //Up-Right
                Debug.LogWarning($"{this}[BridgeTile]: Up-Right");
                prefab = GridManager.Instance.register.get_bridge_corner();
                facing = Quaternion.Euler(0, 270, 0);
                break;
            case (false, true, true, false): //Down-Left
                Debug.LogWarning($"{this}[BridgeTile]: Down-Left");
                prefab = GridManager.Instance.register.get_bridge_corner();
                facing = Quaternion.Euler(0, 90, 0);
                break;
            case (false, true, false, true): //Down-Right
                Debug.LogWarning($"{this}[BridgeTile]: Down-Right");
                prefab = GridManager.Instance.register.get_bridge_corner();
                facing = Quaternion.Euler(0, 180, 0);
                break;
            case (true, false, false, false): //Up
                prefab = GridManager.Instance.register.get_bridge_line();
                facing = Quaternion.Euler(0, 270, 0);
                Debug.LogWarning($"{this}[BridgeTile]: Up");
                break;
            case (false, true, false, false): //Down
                prefab = GridManager.Instance.register.get_bridge_line();
                facing = Quaternion.Euler(0, 90, 0);
                Debug.LogWarning($"{this}[BridgeTile]: Down");
                break;
            case (false, false, true, false): //Left
                prefab = GridManager.Instance.register.get_bridge_line();
                Debug.LogWarning($"{this}[BridgeTile]: Left");
                break;
            case (false, false, false, true): //Right
                prefab = GridManager.Instance.register.get_bridge_line();
                facing = Quaternion.Euler(0, 180, 0);
                Debug.LogWarning($"{this}[BridgeTile]: Right");
                break;
            default: //Individual platform
                Debug.LogWarning($"{this}[BridgeTile]: Platform");
                prefab = GridManager.Instance.register.get_bridge_point();
                break;
        }
        Vector3 locate = transform.position;
        locate.y -= 0.27f;
        GameObject wall = Instantiate(prefab, locate, facing);
        wall.transform.parent = gameObject.transform;
    }
}
