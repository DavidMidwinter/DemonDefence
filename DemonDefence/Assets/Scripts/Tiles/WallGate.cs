using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTile : Ground
{
    new public void Awake()
    {
        GridManager.UpdateTiles += setTile;
        base.Awake();
    }
    public new void OnDestroy()
    {
        GridManager.UpdateTiles -= setTile;
        base.OnDestroy();
    }

    private bool checkTileIsWall(string tileName)
    {
        return tileName.Equals("WallTile") || tileName.Equals("GateTile");
    }
    public void setTile()
    {
        Debug.LogWarning(gameObject.name);
        GameObject prefab;
        Quaternion facing = Quaternion.identity;
        string upTile = neighbours.Find(u => u.get2dLocation().y == get2dLocation().y + 10).GetType().ToString();
        string leftTile = neighbours.Find(u => u.get2dLocation().x == get2dLocation().x + 10).GetType().ToString();

        bool up = checkTileIsWall(upTile);
        bool left = checkTileIsWall(leftTile);
        if (up)
        {
            prefab = GridManager.Instance.register.get_gate_wall();
            facing = Quaternion.Euler(0, 90, 0);
        }
        else if (left)
        {
            prefab = GridManager.Instance.register.get_gate_wall();
        }
        else
        {
            prefab = new GameObject();
        }
        GameObject wall = Instantiate(prefab, transform.position, facing);
        wall.transform.parent = gameObject.transform;
    }


}
