using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField]
    private buildingType thisType;
    private List<TileSlot> buildingTiles;
    [SerializeField]
    public Vector2[] validTiles;
    private Vector2 origin;
    public bool isCoreBuilding;

    [SerializeField]
    private string buildingName;
    [SerializeField]
    // Start is called before the first frame update
    void Awake()
    {
        buildingTiles = new List<TileSlot>();
    }

    public void instantiateBuilding(Vector2 origin_point)
    {
        origin = origin_point;
        foreach(Vector2 tileOffset in validTiles)
        {
            Vector2 tileLocation = origin_point + tileOffset;
            TileSlot tile = GridManager.Instance.getTile(tileLocation);
            if (tile == null) {
                Debug.LogError($"Tile {tileLocation} does not exist");
                Destroy(this);
                return;
            }
            buildingTiles.Add(tile);
            if (tile.getBuilding() != null) Destroy(tile.getBuilding().gameObject);
            tile.setBuilding(this);
        }
        if (isCoreBuilding && GridManager.Instance.coreBuilding != null)
            Destroy(GridManager.Instance.coreBuilding.gameObject);

        GridManager.Instance.addBuilding(this);
    }
    public buildingType getBuilding()
    {
        return thisType;
    }

    public void OnDestroy()
    {
        foreach (TileSlot tile in buildingTiles)
            tile.setBuilding();
        GridManager.Instance.removeBuilding(this);
    }
    public Vector2 getOrigin()
    {
        return origin;
    }

    public string getName()
    {
        return buildingName;
    }

    public int getKey()
    {
        return (int)thisType;
    }
}

public enum buildingType
{
    building2x2 = 0,
    building2x1 = 1,
    building1x2 = 2,
    coreChurch = 3
}
