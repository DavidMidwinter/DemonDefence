using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TacticalUI : MonoBehaviour
{
    /// <summary>
    /// Control all UI functionality for the Tactical UI
    /// </summary>
    public static TacticalUI Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    private Button skipButton;
    public bool mouseOnUI;
    public bool generated;
    VisualElement root => _document.rootVisualElement;
    VisualElement turnDisplay => root.Q<VisualElement>(className: "turn-display");
    VisualElement rollDisplay => root.Q<VisualElement>(className: "result-cards");
    VisualElement actionDisplay => root.Q<VisualElement>(className: "actions");
    int UILayer;

    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += GameManagerStateChanged;


        skipButton = UITools.Create("End\nTurn", endTurn, "skip-button", "player");
        generated = false;
        UILayer = 5;


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        skipButton = UITools.Create("End\nTurn", endTurn, "skip-button", "player");
        generated = false;
        GenerateTurnUI();
        PopulateTurnUI("default");

    }
    private void endTurn()
    {
        Debug.Log("end turn");
        if (GameManager.Instance.State != GameState.PlayerTurn) return;
        UnitManager.Instance.SetSelectedHero(null);
        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
    }
    private IEnumerator GenerateTurnUI()
    {
        /// Generate the Turn UI
        /// Args:
        ///     string faction: The faction whose turn to generate. Default null.
        Debug.Log($"Generate turn UI");
        yield return null;
        root.Clear();
        mouseOnUI = false;

        root.styleSheets.Add(_styleSheet);

        var rollDisplay = UITools.Create("roll-board", "result-cards");
        var actionDisplay = UITools.Create("action-board","actions");

        root.Add(UITools.Create("turn-display"));

        root.Add(rollDisplay);
        rollDisplay.style.display = DisplayStyle.None;
        root.Add(actionDisplay);
        actionDisplay.style.display = DisplayStyle.None;
        root.Add(skipButton);
        generated = true;
    }

    private IEnumerator PopulateTurnUI(string faction = null)
    {

        Debug.Log($"Populate {faction} UI");
        while (!generated)
            yield return null;

        turnDisplay.Clear();
        actionDisplay.Clear();
        rollDisplay.Clear();

        if (faction != null)
        {
            var turnTextBox = UITools.Create<TextElement>("turn-text-box", faction.ToLower());
            turnTextBox.text = $"{faction} Turn";
            turnDisplay.Add(turnTextBox);
        }

        if (faction.ToLower() == "player") UnitManager.Instance.StartTurn(Faction.Player);
        if (faction.ToLower() == "enemy") UnitManager.Instance.StartTurn(Faction.Enemy);


    }

    public void addAction(string buttonText, Action method)
    {
        Button btn = UITools.Create(buttonText, method, "player", "action-button");
        Debug.Log(actionDisplay);
        actionDisplay.style.display = DisplayStyle.Flex;
        actionDisplay.Add(btn);

        
    }

    public void clearActions()
    {
        mouseOnUI = false;
        actionDisplay.Clear();
    }
    private IEnumerator GenerateEndUI(string faction = null)
    {
        /// Generates the end UI
        /// Args:
        ///     string faction: The victorious faction, default null
        Debug.Log($"Generate {faction} victory");
        yield return null;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = UITools.Create("container");

        var resultDisplay = UITools.Create("turn-display");

        if (faction != null)
        {
            var result = UITools.Create<TextElement>("turn-text-box", faction.ToLower());
            resultDisplay.Add(result); 
            if (faction == "Player")
            {
                result.text = "Victory";
            }
            else if (faction == "Enemy")
            {
                result.text = "Defeat";
            }
        }
        container.Add(resultDisplay);

        root.Add(container);

    }

    public void DisplayResults((int result, bool pass)[] results)
    {
        /// Display a set of dice results
        /// Args:
        ///     int[] results: The results to display
        
        for (int index = 0; index < results.Length; index++)
        {
            string cardColour = results[index].pass ? "green" : "red";
            var diceCard = UITools.Create("roll-card", cardColour);
            var diceCardText = UITools.Create<TextElement>("roll-unit");
            diceCardText.text = $"{results[index].result}";
            diceCard.Add(diceCardText);
            rollDisplay.Add(diceCard);
        }
        rollDisplay.style.display = DisplayStyle.Flex;
    }
    public void ClearResults()
    {
        /// Clear dice results from the screen
        rollDisplay.Clear();
        rollDisplay.style.display = DisplayStyle.None;
    }
    private void GameManagerStateChanged(GameState state)
    {
        /// Call UI generation on game state change
        switch (state)
        {
            case GameState.InstructionPage:
                break;
            case GameState.CreateGrid:
                StartCoroutine(GenerateTurnUI());
                break;
            case GameState.SpawnPlayer:
                break;
            case GameState.SpawnEnemy:
                break;
            case GameState.PlayerTurn:
                StartCoroutine(PopulateTurnUI("Player"));
                enableSkip();
                break;
            case GameState.EnemyTurn:
                StartCoroutine(PopulateTurnUI("Enemy"));
                disableSkip();
                break;
            case GameState.Victory:
                StartCoroutine(GenerateEndUI("Player"));
                disableSkip();
                break;
            case GameState.Defeat:
                StartCoroutine(GenerateEndUI("Enemy"));
                disableSkip();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void enableSkip()
    {
        skipButton.SetEnabled(true);
        skipButton.style.display = DisplayStyle.Flex;
    }

    public void disableSkip()
    {
        skipButton.SetEnabled(false);
        skipButton.style.display = DisplayStyle.None;
    }

    public void enableOrders()
    {
        VisualElement display = root.Q<VisualElement>(className: "actions");
        display.style.display = DisplayStyle.Flex;
    }

    public void disableOrders()
    {
        VisualElement display = root.Q<VisualElement>(className: "actions");
        display.style.display = DisplayStyle.None;
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        EventSystem.current.gameObject.transform.Find("PanelSettings").gameObject.layer = UILayer;
        bool isOver = IsPointerOverUIElement(GetEventSystemRaycastResults());
        Debug.LogWarning(isOver);
        return isOver;
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        Debug.LogWarning(eventSystemRaysastResults.Count);
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            Debug.LogWarning($"{curRaysastResult.gameObject.layer} / {UILayer}");
            if (curRaysastResult.gameObject.layer == UILayer)
            {
                return true;
            }
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

}

