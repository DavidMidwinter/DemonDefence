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
    [SerializeField] private Texture2D eraseIcon, editSpawnIcon;

    private Dictionary<Faction, UnitType[]> factionTypes = new Dictionary<Faction, UnitType[]> {
        { Faction.Enemy, new UnitType[]{UnitType.Cultist, UnitType.Demonic, UnitType.Despoiler} },
        { Faction.Player, new UnitType[]{UnitType.Common, UnitType.Pious, UnitType.Mechanised} },
    };
    VisualElement root => _document.rootVisualElement;
    VisualElement tileBoard => root.Q<VisualElement>(className: "tile-board");
    VisualElement buildingBoard => root.Q<VisualElement>(className: "building-board");
    VisualElement spawnBoard => root.Q<VisualElement>(className: "spawn-board");
    VisualElement spawnEditor => root.Q<VisualElement>(className: "spawn-editor");
    Label spawnName => spawnEditor.Q<Label>(className: "spawn-editor-spawn-name");
    VisualElement spawnCheckboxes => spawnEditor.Q<VisualElement>(className: "spawn-editor-checkboxes");

    DropdownField spawnMapList => root.Q<DropdownField>(className: "spawn-map-list");

    private int UILayer;

    private void Awake()
    {
        Instance = this;
        UILayer = 5;
        StartCoroutine(GenerateUI());
        StartCoroutine(PopulateUI());
    }

    private void Start()
    {
        TileSlot.checkSpawnRadius += updateSpawnWindow;
        BrushManager.onBrushStateChanged += onBrushChange;
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(GenerateUI());
    }

    private void OnDestroy()
    {
        TileSlot.checkSpawnRadius -= updateSpawnWindow;

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
        root.Add(createSpawnEditWindow());


    }

    public IEnumerator PopulateUI()
    {
        yield return 0;
        VisualElement upperBoard = UITools.Create("upper-board");
        upperBoard.Add(UITools.Create("Save File", GridManager.Instance.saveMap, "save-button"));
        upperBoard.Add(createSpawnmapDropdown("Spawnmaps:", null, "spawn-map-list"));
        upperBoard.Add(UITools.Create("New Spawnmap", addSpawnMap, "new-spawn-map"));

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
        root.Add(upperBoard);
        spawnBoard.Add(createResourceButton("erase", spawnEraser, eraseIcon, resourceType.spawnpoint));
        spawnBoard.Add(createResourceButton("edit", spawnEdit, editSpawnIcon, resourceType.spawnpoint));
        spawnBoard.Add(createSpawnpointButton(GridManager.Instance.playerSpawnPrefab));
        spawnBoard.Add(createSpawnpointButton(GridManager.Instance.enemySpawnPrefab));
        showTileBoard();
        while (spawnEditor is null) yield return null;
        disableSpawnWindow();
        StartCoroutine(PopulateSpawnmapDropdown());
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
        BrushManager.Instance.clearSpawnSelect();
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
        BrushManager.Instance.clearSpawnSelect();
        BrushManager.Instance.state = brushState.deleteSpawnpoint;

    }

    private void spawnEdit()
    {
        BrushManager.Instance.state = brushState.editSpawnpoint;
    }

    public void hideUI()
    {
        root.style.display = DisplayStyle.None;
    }

    public void showUI()
    {
        root.style.display = DisplayStyle.Flex;
    }

    private VisualElement createSpawnEditWindow()
    {
        VisualElement window = UITools.Create("spawn-editor");

        Label title = UITools.Create<Label>("spawn-editor-title");
        title.text = "Edit Spawnpoint";
        window.Add(title);

        Label spawnName = UITools.Create<Label>("spawn-editor-spawn-name");
        window.Add(spawnName);

        Label spawnTypes = UITools.Create<Label>("spawn-editor-type-label");

        spawnTypes.text = "Allowed units (if none selected, allows all)";
        window.Add(spawnTypes);

        VisualElement typeCheckboxes = UITools.Create("spawn-editor-checkboxes");

        window.Add(typeCheckboxes);

        return window;
    }

    private void updateSpawnWindow()
    {
        if (BrushManager.Instance.state != brushState.editSpawnpoint || BrushManager.Instance.selectedToEdit is null)
        {
            disableSpawnWindow();
            return;
        }
        enableSpawnWindow();
        spawnName.text = BrushManager.Instance.selectedToEdit.name;
        spawnCheckboxes.Clear();

        foreach(UnitType unitType in factionTypes[BrushManager.Instance.selectedToEdit.faction])
        {
            spawnCheckboxes.Add(createUnitTypeCheckbox(
                unitType, 
                BrushManager.Instance.selectedToEdit.spawnpointData.validUnits.Contains(unitType)
                ));
        }

    }
    private void onBrushChange()
    {
        if (BrushManager.Instance.state != brushState.editSpawnpoint)
            disableSpawnWindow();
    }
    public static VisualElement createUnitTypeCheckbox(UnitType unitType, bool defaultValue)
    {
        VisualElement settingDisplay = UITools.Create("setting-display");
        Label settingName = UITools.Create<Label>();
        settingName.text = unitType.ToString();

        Toggle text = UITools.Create<Toggle>(unitType.ToString());
        text.value = defaultValue;
        text.RegisterValueChangedCallback(evt => toggleUnitType(unitType, evt.newValue));
        toggleUnitType(unitType, defaultValue);
        settingDisplay.Add(settingName);
        settingDisplay.Add(text);
        return settingDisplay;
    }

    void enableSpawnWindow()
    {
        spawnEditor.style.display = DisplayStyle.Flex;
    }
    void disableSpawnWindow()
    {
        spawnEditor.style.display = DisplayStyle.None;
    }
    private DropdownField createSpawnmapDropdown(string name, string initial, string lookup, string displayclass = "dropdown-menu")
    {
        DropdownField dropDown = UITools.CreateDropdown(name, initial, lookup, displayclass, "instruction-text");

        dropDown.RegisterValueChangedCallback(evt => setSpawnmap(evt.newValue));

        return dropDown;
    }


    private void setSpawnmap(string value)
    {
        GridManager.Instance.loadSpawnmap(value);
    }

    static void toggleUnitType(UnitType unitType, bool value)
    {
        if (value) BrushManager.Instance.selectedToEdit.spawnpointData.validUnits.Add(unitType);
        else BrushManager.Instance.selectedToEdit.spawnpointData.validUnits.Remove(unitType);
    }

    private IEnumerator PopulateSpawnmapDropdown()
    {
        while(spawnMapList is null){
            yield return null;
            Debug.LogWarning("spawnMapList does not exist");
        }

        while(GridManager.Instance.getSpawnIndex() < 0)
        {
            yield return null;
            Debug.LogWarning("GridDataManager is still adding new spawnmap");
        }

        spawnMapList.choices.Clear();
        foreach (string spawnMap in GridManager.Instance.getSpawnMapNames())
            spawnMapList.choices.Add(spawnMap);
        spawnMapList.SetValueWithoutNotify(GridManager.Instance.getSpawnMapNames()[GridManager.Instance.getSpawnIndex()]);
    }
    public void addSpawnMap()
    {
        GridManager.Instance.createNewSpawnMap();
        StartCoroutine(PopulateSpawnmapDropdown());
    }
}

