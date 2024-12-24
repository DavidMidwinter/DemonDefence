using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSlot : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector2 location;
    private tileType typeOfTile;
    private List<TileSlot> neighbours;
    private Building occupyingBuilding;
    private bool canSupportBuilding;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private (float rotationY, float rotationW, float scale) foliage_data;

    public void setLocation(Vector2 coords)
    {
        location = coords;
        gameObject.name = $"{coords}";
    }

    public Vector2 getLocation()
    {
        return location;
    }

    public void addNeighbour(TileSlot neighbour)
    {
        if (neighbours is null) neighbours = new List<TileSlot>();
        neighbours.Add(neighbour);
    }

    public List<TileSlot> getNeighbours()
    {
        return neighbours;
    }
    public void setTileType(Tile newType)
    {
        typeOfTile = newType.thisType;
        canSupportBuilding = newType.canSupportBuilding;
        if (occupyingBuilding is not null && !canSupportBuilding)
            occupyingBuilding.delete();
        spriteRenderer.sprite = newType.tileGraphic;

        if((typeOfTile == tileType.bush))
        {
            foliageRandomiser(0.8f, 1.1f);
        }

        else if ((typeOfTile == tileType.tree))
        {
            foliageRandomiser(1, 1.5f);
        }
    }

    public tileType getType()
    {
        return typeOfTile;
    }

    public bool getSupportBuilding()
    {
        return canSupportBuilding;
    }

    public void setBuilding(Building toSet = null)
    {
        occupyingBuilding = toSet;
    }

    public Building getBuilding()
    {
        return occupyingBuilding;
    }

    public bool hasBuilding()
    {
        return occupyingBuilding != null;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (PaintUI.Instance.IsPointerOverUIElement())
            {
                Debug.LogWarning("Mouse on UI button");
                return;
            }
            setTileType(BrushManager.Instance.selectedTile);

        }
    }

    public void foliageRandomiser(float minScale, float maxScale)
    {
        Quaternion randomRotation = Random.rotation;
        foliage_data.rotationY = randomRotation.y;
        foliage_data.rotationW = randomRotation.w;
        foliage_data.scale = Random.Range(minScale, maxScale) * 2.5f;

    }

    public void setFoliageData(float rotationY, float rotationW, float scale)
    {
        foliage_data = (rotationY, rotationW, scale);
    }

    public (float rotationY, float rotationW, float scale) getFoliageData()
    {
        return foliage_data;
    }
}
