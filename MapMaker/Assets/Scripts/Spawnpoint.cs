using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointObject : MonoBehaviour
{
    public Faction faction;
    public SpriteRenderer spriteRenderer;
    public Color spawnColor;
    public Spawnpoint spawnpointData;
    private TileSlot _tile;


    public SpawnpointObject(Vector2 location)
    {
        spawnpointData = new Spawnpoint(location);
        setLocation(location);
    }
    public SpawnpointObject(Spawnpoint spawn)
    {
        setSpawnpointData(spawn);
    }
    public void setLocation(Vector2 location)
    {
        transform.position = new Vector3(location.x, location.y, -3);
        spawnpointData.location = location;
        _tile = GridManager.Instance.getTile(location);
        _tile.setSpawnRef(this);
    }

    public void setSpawnpointData(Spawnpoint data)
    {
        spawnpointData = data;
        setLocation(spawnpointData.location);
    }

    public void initData(Spawnpoint data)
    {
        setSpawnpointData(data);
        setName();
    }

    public void initData(Vector2 location)
    {
        setLocation(location);
        setName();
    }
    public void setName()
    {
        name = $"{faction} spawn {spawnpointData.location}";
    }
    public Spawnpoint getSpawnpointData()
    {
        return spawnpointData;
    }

    public Vector2 getLocation()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void setBrush()
    {
        BrushManager.Instance.selectedSpawn = this;
        BrushManager.Instance.state = brushState.placeSpawnpoint;
        BrushManager.Instance.clearSpawnSelect();
        TileSlot.callTileCheck();
    }

    public void OnDestroy()
    {
        if (_tile)
            _tile.setSpawnRef();
        GridManager.Instance.removeSpawn(this);
        TileSlot.callTileCheck();
    }

}

public enum Faction
{
    Player = 0,
    Enemy = 1
}
