using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTile : Ground
{
    [SerializeField]
    private (Vector2 outside, Vector2 inside) accessTiles;
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
        string leftTile = neighbours.Find(u => u.get2dLocation().x == get2dLocation().x + 10).GetType().ToString();

        bool up = checkTileIsWall(upTile);
        bool left = checkTileIsWall(leftTile);
        if (up)
        {
            prefab = GridManager.Instance.register.get_gate_wall();
            facing = Quaternion.Euler(0, 90, 0);

            if(get2dLocation().y > GridManager.Instance.centrepoint.y)
            {
                accessTiles.outside.y = get2dLocation().y + 10;
                accessTiles.inside.y = get2dLocation().y - 10;
            }
            else
            {
                accessTiles.outside.y = get2dLocation().y - 10;
                accessTiles.inside.y = get2dLocation().y + 10;
            }
            accessTiles.outside.x = get2dLocation().x;
            accessTiles.inside.x = get2dLocation().x;
        }
        else if (left)
        {
            prefab = GridManager.Instance.register.get_gate_wall();

            if (get2dLocation().x > GridManager.Instance.centrepoint.x)
            {
                accessTiles.outside.x = get2dLocation().x + 10;
                accessTiles.inside.x = get2dLocation().x - 10;
            }
            else
            {
                accessTiles.outside.x = get2dLocation().x - 10;
                accessTiles.inside.x = get2dLocation().x + 10;
            }
            accessTiles.outside.y = get2dLocation().y;
            accessTiles.inside.y = get2dLocation().y;
        }
        else
        {
            prefab = new GameObject();
        }
        GameObject wall = Instantiate(prefab, transform.position, facing);
        wall.transform.parent = gameObject.transform;
    }

    public Tile getOuterTile()
    {
        return neighbours.Find(t => t.get2dLocation() == accessTiles.outside);
    }
    public Tile getInnerTile()
    {
        return neighbours.Find(t => t.get2dLocation() == accessTiles.inside);
    }
}
