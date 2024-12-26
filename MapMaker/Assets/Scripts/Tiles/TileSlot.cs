using System;
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


    [SerializeField]
    private GameObject highlight;

    public static Action checkSpawnRadius;

    public SpriteRenderer spawnHighlight;   



    public void Awake()
    {
        highlight.SetActive(false);
        spawnHighlight.gameObject.SetActive(false);
        BrushManager.onBrushStateChanged += checkSpawnRadiusMethod;
        checkSpawnRadius += checkSpawnRadiusMethod;
    }

    public void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    public void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButton(0) && BrushManager.Instance.state == brushState.paintTiles)
        {
            if (PaintUI.Instance.IsPointerOverUIElement())
            {
                Debug.LogWarning("Mouse on UI button");
                return;
            }
            setTileType(BrushManager.Instance.selectedTile);

        }
    }

    public void OnMouseDown()
    {
        if (PaintUI.Instance.IsPointerOverUIElement())
        {
            Debug.LogWarning("Mouse on UI button");
            return;
        }
        switch (BrushManager.Instance.state)
        {
            case brushState.paintTiles:
                return;
            case brushState.placeBuilding:
                placeBuilding(BrushManager.Instance.selectedBuilding.prefab);
                break;
            case brushState.deleteBuilding:
                if (occupyingBuilding is not null) Destroy(occupyingBuilding.gameObject);
                break;
            case brushState.placeCoreBuilding:
                break;
            case brushState.placeSpawnpoint:
                if (occupyingBuilding is null && BrushManager.Instance.selectedSpawn is not null)
                {
                    BrushManager.Instance.selectedSpawn.setLocation(location);
                    checkSpawnRadius?.Invoke();
                }
                break;
            default:
                break;
        }
    }
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
            Destroy(occupyingBuilding.gameObject);
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
        Debug.Log(occupyingBuilding != null);
        return occupyingBuilding != null;
    }

    public void foliageRandomiser(float minScale, float maxScale)
    {
        Quaternion randomRotation = UnityEngine.Random.rotation;
        foliage_data.rotationY = randomRotation.y;
        foliage_data.rotationW = randomRotation.w;
        foliage_data.scale = UnityEngine.Random.Range(minScale, maxScale) * 2.5f;

    }

    public void setFoliageData(float rotationY, float rotationW, float scale)
    {
        foliage_data = (rotationY, rotationW, scale);
    }

    public (float rotationY, float rotationW, float scale) getFoliageData()
    {
        return foliage_data;
    }



    public bool checkBuilding(Building toPlace)
    {
        foreach(Vector2 offset in toPlace.validTiles)
        {
            TileSlot tileLoc = GridManager.Instance.getTile(location + offset);
            if (tileLoc is null || tileLoc.canSupportBuilding is false) return false;
        }
        return true;
    }

    public void placeBuilding(Building toPlace)
    {
        Building newBuilding = Instantiate(toPlace);
        if (!checkBuilding(newBuilding)) {
            Destroy(newBuilding.gameObject);
            return; 
        }
        newBuilding.transform.position = new Vector3(location.x, location.y, -1f);
        newBuilding.instantiateBuilding(location);
        
    }

    public void checkSpawnRadiusMethod()
    {
        if(BrushManager.Instance.state != brushState.placeSpawnpoint)
        {
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
        if (occupyingBuilding is not null)
        {
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
        if(Utils.calculateDistance(location, GridManager.Instance.getSpawn(Faction.Player).getLocation()) <= GridManager.Instance.getSpawnRadius())
        {
            Debug.Log($"{this} is player spawn tile");
            spawnHighlight.gameObject.SetActive(true);
            spawnHighlight.color = GridManager.Instance.getSpawn(Faction.Player).spawnColor;
            return;
        }
        else if (Utils.calculateDistance(location, GridManager.Instance.getSpawn(Faction.Enemy).getLocation()) <= GridManager.Instance.getSpawnRadius())
        {
            Debug.Log($"{this} is enemy spawn tile");
            spawnHighlight.gameObject.SetActive(true);
            spawnHighlight.color = GridManager.Instance.getSpawn(Faction.Enemy).spawnColor;
            return;
        }

        else
        {
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
    }
}
