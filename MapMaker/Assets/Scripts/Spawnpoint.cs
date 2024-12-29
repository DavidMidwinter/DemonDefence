using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointObject : MonoBehaviour
{
    public Faction faction;
    public SpriteRenderer spriteRenderer;
    public Color spawnColor;
    public Spawnpoint spawnpointData;


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
    }

    public void setSpawnpointData(Spawnpoint data)
    {
        spawnpointData = data;
        transform.position = new Vector3(data.location.x, data.location.y, -3);
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
    }

}

public enum Faction
{
    Player = 0,
    Enemy = 1
}
