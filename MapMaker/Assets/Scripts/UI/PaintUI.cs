using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PaintUI : MonoBehaviour
{
    public static PaintUI Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    [SerializeField] private Texture2D eraseIcon;
    VisualElement root => _document.rootVisualElement;
    VisualElement tileBoard => root.Q<VisualElement>(className: "tile-board");
    VisualElement buildingBoard => root.Q<VisualElement>(className: "building-board");
    VisualElement spawnBoard => root.Q<VisualElement>(className: "spawn-board");

    private int UILayer;

    private void Awake()
    {
        Instance = this;
        UILayer = 5;
        StartCoroutine(GenerateUI());
        StartCoroutine(PopulateUI());
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(GenerateUI());
    }

    private IEnumerator GenerateUI()
    {
        Debug.Log("Generate paint UI");
        yield return null;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        Debug.Log("Create board display");
        VisualElement board = UITools.Create("board-page");
        VisualElement navButtons = UITools.Create("nav-buttons");

        navButtons.Add(UITools.Create("Tiles", showTileBoard, "nav-button"));

        navButtons.Add(UITools.Create("Buildings", showBuildingBoard, "nav-button"));

        navButtons.Add(UITools.Create("Spawns", showSpawnBoard, "nav-button"));


        board.Add(navButtons);
        Debug.Log("Create tile board");
        board.Add(UITools.Create("tile-board", "button-display"));
        Debug.Log("Create building board");
        board.Add(UITools.Create("building-board", "button-display"));
        Debug.Log("Create spawn board");
        board.Add(UITools.Create("spawn-board", "button-display"));

        root.Add(board);


    }

    public IEnumerator PopulateUI()
    {
        yield return 0;
        root.Add(UITools.Create("Save File", GridManager.Instance.saveMap, "save-button"));

        foreach (Tile tile in TileManager.Instance.getAllTiles())
        {
            Debug.Log($"Create button for {tile.thisType}");
            tileBoard.Add(createTileButton(tile));
        }


        buildingBoard.Add(createResourceButton("erase", buildingEraser, eraseIcon, resourceType.building));
        foreach (BuildingTemplate building in BuildingManager.Instance.getAllBuildings())
        {
            Debug.Log($"Create button for {building.thisType}");
            buildingBoard.Add(createBuildingButton(building));
        }

        spawnBoard.Add(createResourceButton("erase", spawnEraser, eraseIcon, resourceType.spawnpoint));
        spawnBoard.Add(createSpawnpointButton(GridManager.Instance.playerSpawnPrefab));
        spawnBoard.Add(createSpawnpointButton(GridManager.Instance.enemySpawnPrefab));
        showTileBoard();
    }

    public bool IsPointerOverUIElement()
    {
        EventSystem.current.gameObject.transform.Find("PanelSettings").gameObject.layer = UILayer;
        bool isOver = IsPointerOverUIElement(GetEventSystemRaycastResults());
        return isOver;
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        Debug.Log(eventSystemRaysastResults.Count);
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            Debug.Log($"{curRaysastResult.gameObject.layer} / {UILayer}");
            if (curRaysastResult.gameObject.layer == UILayer)
            {
                return true;
            }
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    
    public Button createTileButton(Tile tile)
    {
        return createResourceButton(tile.thisType.ToString(), tile.setBrush,
        tile.tileGraphic.texture, resourceType.tile);
    }

    public Button createBuildingButton(BuildingTemplate building)
    {
        return createResourceButton(building.thisType.ToString(), building.setBrush,
        building.buildingGraphic.texture, resourceType.building);
    }

    public Button createSpawnpointButton(SpawnpointObject spawnpoint)
    {
        return createResourceButton(
            spawnpoint.faction.ToString(),
            spawnpoint.setBrush,
            spawnpoint.spriteRenderer.sprite.texture,
            resourceType.spawnpoint);
    }

    private Button createResourceButton(string resourceName, Action methodToCall, Texture2D texture, resourceType resourceType)
    {
        Button button = UITools.Create(null, methodToCall, $"{resourceName}-{resourceType}", "tile-button");

        Image img = UITools.Create<Image>("button-image");
        img.scaleMode = ScaleMode.ScaleToFit;
        img.image = texture;
        Label name = UITools.Create<Label>("label", "resource-label");
        name.text = resourceName;

        button.Add(img);
        button.Add(name);

        Debug.Log(button);
        return button;
    }
    public void empty() { }
    public void showTileBoard()
    {
        tileBoard.style.display = DisplayStyle.Flex;
        buildingBoard.style.display = DisplayStyle.None;
        spawnBoard.style.display = DisplayStyle.None;
        BrushManager.Instance.setBrush(brushState.paintTiles);
    }
    public void showBuildingBoard()
    {
        tileBoard.style.display = DisplayStyle.None;
        buildingBoard.style.display = DisplayStyle.Flex;
        spawnBoard.style.display = DisplayStyle.None;
        BrushManager.Instance.setBrush(brushState.placeBuilding);
    }
    public void showSpawnBoard()
    {
        tileBoard.style.display = DisplayStyle.None;
        buildingBoard.style.display = DisplayStyle.None;
        spawnBoard.style.display = DisplayStyle.Flex;
        BrushManager.Instance.setBrush(brushState.placeSpawnpoint);
    }
    private enum resourceType
    {
        tile,
        building,
        spawnpoint
    }

    private void buildingEraser()
    {
        BrushManager.Instance.state = brushState.deleteBuilding;
    }

    private void spawnEraser()
    {
        BrushManager.Instance.state = brushState.deleteSpawnpoint;

    }

    public void hideUI()
    {
        root.style.display = DisplayStyle.None;
    }

    public void showUI()
    {
        root.style.display = DisplayStyle.Flex;
    }
}

