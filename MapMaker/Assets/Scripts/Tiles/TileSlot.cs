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
    [SerializeField]
    private GameObject blocker;

    public static Action checkSpawnRadius;

    public SpriteRenderer spawnHighlight;

    [SerializeField]
    private SpawnpointObject occupyingSpawn;



    public void Awake()
    {
        highlight.SetActive(false);
        spawnHighlight.gameObject.SetActive(false);
        blocker.SetActive(false);
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
                deleteBuilding();
                break;
            case brushState.placeCoreBuilding:
                break;
            case brushState.placeSpawnpoint:
                if (occupyingBuilding is null)
                {
                    placeSpawn(BrushManager.Instance.selectedSpawn);
                    checkSpawnRadius?.Invoke();
                }
                break;
            case brushState.deleteSpawnpoint:
                deleteSpawn();
                break;
            case brushState.editSpawnpoint:
                selectSpawn();
                break;
            default:
                break;
        }
    }
    public void OnDestroy()
    {
        BrushManager.onBrushStateChanged -= checkSpawnRadiusMethod;
        checkSpawnRadius -= checkSpawnRadiusMethod;

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

    public void placeSpawn(SpawnpointObject toPlace)
    {
        Debug.Log($"occupied: {occupyingSpawn is null}");
        if (occupyingBuilding is null 
            && !GridManager.Instance.notWalkable.Contains(typeOfTile)
            && occupyingSpawn is null)
        {
            SpawnpointObject newSpawn = Instantiate(toPlace);
            newSpawn.initData(location);
            GridManager.Instance.addSpawn(newSpawn);
            checkSpawnRadius?.Invoke();
        }
    }
    public void checkSpawnRadiusMethod()
    {
        if(!(BrushManager.Instance.state == brushState.placeSpawnpoint || BrushManager.Instance.state == brushState.deleteSpawnpoint || BrushManager.Instance.state == brushState.editSpawnpoint))
        {
            blocker.gameObject.SetActive(false);
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
        if (occupyingBuilding is not null)
        {
            blocker.gameObject.SetActive(true);
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
        if (GridManager.Instance.notWalkable.Contains(typeOfTile))
        {
            blocker.gameObject.SetActive(true);
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
        if (BrushManager.Instance.state == brushState.editSpawnpoint
            && BrushManager.Instance.selectedToEdit is not null
            && Utils.calculateDistance(location, BrushManager.Instance.selectedToEdit.getLocation()) <= GridManager.Instance.getSpawnRadius())
        {
            Debug.Log($"{this} is selected spawn tile");
            blocker.gameObject.SetActive(false);
            spawnHighlight.gameObject.SetActive(true);
            spawnHighlight.color = Color.white;
            return;
        }
        else if (GridManager.Instance.getSpawns(Faction.Player).Exists(x =>Utils.calculateDistance(location, x.getLocation()) <= GridManager.Instance.getSpawnRadius()))
        {
            Debug.Log($"{this} is player spawn tile");
            blocker.gameObject.SetActive(false);
            spawnHighlight.gameObject.SetActive(true);
            spawnHighlight.color = GridManager.Instance.getSpawn(Faction.Player).spawnColor;
            return;
        }
        else if (GridManager.Instance.getSpawns(Faction.Enemy).Exists(x => Utils.calculateDistance(location, x.getLocation()) <= GridManager.Instance.getSpawnRadius()))
        {
            Debug.Log($"{this} is enemy spawn tile");
            blocker.gameObject.SetActive(false);
            spawnHighlight.gameObject.SetActive(true);
            spawnHighlight.color = GridManager.Instance.getSpawn(Faction.Enemy).spawnColor;
            return;
        }

        else
        {
            blocker.gameObject.SetActive(false);
            spawnHighlight.gameObject.SetActive(false);
            return;
        }
    }

    public void setSpawnRef(SpawnpointObject spawn = null)
    {
        occupyingSpawn = spawn;
    }

    void deleteBuilding()
    {
        if (occupyingBuilding is not null) Destroy(occupyingBuilding.gameObject);
    }
    void deleteSpawn()
    {
        Debug.Log(occupyingSpawn);
        if (occupyingSpawn is not null) Destroy(occupyingSpawn.gameObject);
    }
    void selectSpawn()
    {
        Debug.Log(occupyingSpawn);
        if (occupyingSpawn is not null) BrushManager.Instance.selectSpawn(occupyingSpawn);    }

    public static void callTileCheck()
    {
        checkSpawnRadius?.Invoke();
    }
}
