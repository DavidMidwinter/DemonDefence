using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    private buildingType buildingType;
    private List<TileSlot> buildingTiles;
    private List<Vector2> validTiles;
    private Vector2 origin;
    // Start is called before the first frame update
    void Start()
    {
        buildingTiles = new List<TileSlot>();
    }

    public void instantiateBuilding(Vector2 origin_point)
    {
        origin = origin_point;

    }
    public buildingType getBuilding()
    {
        return buildingType;
    }

    public void delete()
    {

    }
}

public enum buildingType
{
    building1x2,
    building2x1,
    building2x2,
    church
}
