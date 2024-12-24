using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PaintUI : MonoBehaviour
{
    public static PaintUI Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    VisualElement root => _document.rootVisualElement;
    VisualElement tileBoard => root.Q<VisualElement>(className: "tile-board");
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
        Debug.Log("Create tile board");
        root.Add(UITools.Create("tile-board"));
    }

    public IEnumerator PopulateUI()
    {
        yield return 0;
        root.Add(UITools.Create("Save File", GridManager.Instance.saveMap, "save-button"));

        foreach (Tile tile in TileManager.Instance.getAllTiles())
        {
            tileBoard.Add(createTileButton(tile));
        }
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
        Button tileButton = UITools.Create(null, tile.setBrush, $"{tile.thisType}-tile", "tile-button");

        Image img = UITools.Create<Image>("button-image");
        img.scaleMode = ScaleMode.ScaleToFit;
        img.image = tile.tileGraphic.texture;
        Label name = UITools.Create<Label>("label");
        name.text = tile.thisType.ToString();

        tileButton.Add(img);
        tileButton.Add(name);

        return tileButton;
    }
}
