using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public Faction faction;
    public SpriteRenderer spriteRenderer;
    public Color spawnColor;


    public Spawnpoint(Vector2 location)
    {
        setLocation(location);
    }
    public void setLocation(Vector2 location)
    {
        transform.position = new Vector3(location.x, location.y, -3);
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
