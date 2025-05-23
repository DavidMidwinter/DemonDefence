using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    /// <summary>
    /// Functionality to manage the units on the board
    /// </summary>
    public static UnitManager Instance;
    [SerializeField] private int spearmen;
    [SerializeField] private int templars;
    [SerializeField] private int musketeers;
    [SerializeField] private int field_guns;
    [SerializeField] private int organ_guns;


    [SerializeField] private int cultists;
    [SerializeField] private int hellspawn;
    [SerializeField] private int demons;
    [SerializeField] private int kites;
    [SerializeField] private int infernal_engines;

    public List<BasePlayerUnit> allyUnits;
    public List<BaseEnemyUnit> enemyUnits;
    public List<BaseUnit> leaders;


    public BasePlayerUnit SelectedUnit;
    public BaseEnemyUnit SelectedEnemy;

    private List<ScriptableDetachment> _detachments;

    private List<Material> _detachmentColours;

    public Action resetPlayers;
    public Action resetEnemies;

    void Awake()
    {
        Instance = this;
        _detachments = new List<ScriptableDetachment>(Resources.LoadAll<ScriptableDetachment>("Detachments"));
        _detachmentColours = new List<Material>(Resources.LoadAll<Material>("detachmentColours"));
        allyUnits = new List<BasePlayerUnit>();
        enemyUnits = new List<BaseEnemyUnit>();
        leaders = new List<BaseUnit>();
    }

    public void StartTurn(Faction faction)
    {
        /// When the GameState changes, this method is called. 
        /// If the state is the player turn, set all player unit remaining actions to their default
        /// If the state is the enemy turn, set all enemy unit remaining actions to their default, then select the first enemy unit
        Debug.Log("Start Turn");
        if (faction == Faction.Enemy)
        {
            resetEnemies?.Invoke();
            setNextEnemy();
        }
        if(faction == Faction.Player)
        {
            resetPlayers?.Invoke();
            setNextPlayer(forceCamera: true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("tab") 
            && GameManager.Instance.State == GameState.PlayerTurn
            && GameManager.Instance.canInput
            )
        {
            Debug.Log("manually cycle player");
            setNextPlayer(SelectedUnit);
        }
    }

    public void spawnPlayer()
    {
        /// Spawn Player Detachments

        int detachmentColour = 1;

        detachmentColour = spawnPlayerDetachment(DetachmentData.SPEARMEN, spearmen, detachmentColour);

        detachmentColour = spawnPlayerDetachment(DetachmentData.MUSKETS, musketeers, detachmentColour);

        detachmentColour = spawnPlayerDetachment(DetachmentData.FIELD_GUNS, field_guns, detachmentColour);

        detachmentColour = spawnPlayerDetachment(DetachmentData.TEMPLARS, templars, detachmentColour);

        detachmentColour = spawnPlayerDetachment(DetachmentData.ORGAN_GUNS, organ_guns, detachmentColour);


        GameManager.Instance.UpdateGameState(GameState.SpawnEnemy);
    }

    public int spawnPlayerDetachment(string detachmentName, int numberToSpawn, int detachmentColour)
    {
        ScriptableDetachment detachment = DetachmentData.getDetachment(detachmentName);
        for (int i = 0; i < numberToSpawn; i++)
        {
            spawnDetachment(detachment, GridManager.Instance.GetPlayerSpawnTile(detachment.troopUnit.unitPrefab.unitTypes), detachmentColour);
            detachmentColour++;
            if (detachmentColour >= DetachmentData.getAllColours().Count) detachmentColour = 0;
            Debug.Log(detachmentColour);
        }
        return detachmentColour;
    }

    public void spawnDetachment(ScriptableDetachment detachment, Tile origin, int colourIndex)
    {
        /// Spawn a detachment in the player spawn zone
        /// Args:
        ///     ScriptableDetachment detachment: the detachment to spawn

        if (origin is null) return;
        BaseUnit leader = spawnUnit(detachment.leaderUnit, origin, colourIndex);
        leaders.Add(leader);
        leader.name = $"{leader.GetType().Name} {leader.faction}.{colourIndex}.L";
        for (int i = 0; i < detachment.numberOfTroops; i++)
        {
            Tile locTile = GridManager.Instance.GetNearestSpawnableTile(origin);
            if (locTile)
            {
                BaseUnit unit = spawnUnit(detachment.troopUnit, locTile, colourIndex);
                leader.addDetachmentMember(unit);
                unit.name = $"{unit.GetType().Name} {unit.faction}.{colourIndex}.{i}";
            }
        }
    }

    public int spawnEnemyDetachment(string detachmentName, int numberToSpawn, int detachmentColour)
    {
        ScriptableDetachment detachment = DetachmentData.getDetachment(detachmentName);
        for (int i = 0; i < numberToSpawn; i++)
        {
            spawnDetachment(detachment, GridManager.Instance.GetEnemySpawnTile(detachment.troopUnit.unitPrefab.unitTypes), detachmentColour);
            detachmentColour++;
            if (detachmentColour >= DetachmentData.getAllColours().Count) detachmentColour = 0;
            Debug.Log(detachmentColour);
        }
        return detachmentColour;
    }

    public BaseUnit spawnUnit(ScriptableUnit unit, Tile tile, int colourIndex)
    {
        /// Spawn a unit on a given tile, and add unit to their corresponding list
        /// Args:
        ///     BaseUnit unit: THe unit to spawn
        ///     Tile tile: The tile to spawn them on
        var spawnedUnit = Instantiate(unit.unitPrefab);
        spawnedUnit.setDetachmentColour(_detachmentColours[colourIndex]);
        spawnedUnit.transform.position = tile.transform.position;

        tile.SetUnit(spawnedUnit);
        spawnedUnit.setRemainingActions(spawnedUnit.maxActions);
        if (spawnedUnit.GetType().IsSubclassOf(typeof(BasePlayerUnit)))
        {
            allyUnits.Add((BasePlayerUnit)spawnedUnit);
        }

        else if (spawnedUnit.GetType().IsSubclassOf(typeof(BaseEnemyUnit)))
        {
            enemyUnits.Add((BaseEnemyUnit)spawnedUnit);
        }
        spawnedUnit.setUnitNameTag(unit.unitName);
        return spawnedUnit;
    }
    public void spawnEnemy()
    {
        /// Spawn an Enemy Detachment
        
        
        int detachmentColour = 1;

        detachmentColour = spawnEnemyDetachment(DetachmentData.CULTISTS, cultists, detachmentColour);

        detachmentColour = spawnEnemyDetachment(DetachmentData.HELLSPAWN, hellspawn, detachmentColour);

        detachmentColour = spawnEnemyDetachment(DetachmentData.DEMONS, demons, detachmentColour);

        detachmentColour = spawnEnemyDetachment(DetachmentData.KITES, kites, detachmentColour);

        detachmentColour = spawnEnemyDetachment(DetachmentData.INFERNAL_ENGINES, infernal_engines, detachmentColour);

        GameManager.Instance.startGame();
    }

    public void SetSelectedHero(BasePlayerUnit unit)
    {
        /// Set the selected Player unit
        /// Args:
        ///     BasePlayerUnit unit: The unit to select
        Debug.Log($"Select {unit}");
        if (unit && unit.getRemainingActions() <= 0) return;
        TacticalUI.Instance.clearActions();
        if (SelectedUnit) { 
            SelectedUnit.selectionMarker.SetActive(false);
            SelectedUnit.hideHighlight();
        }
        SelectedUnit = unit;
        if (SelectedUnit)
        {
            SelectedUnit.calculateAllTilesInRange();
            SelectedUnit.getAttackTargets();
            SelectedUnit.selectionMarker.SetActive(true);
            SelectedUnit.displayHighlight();
            SelectedUnit.onSelect();
        }

    }

    public void SetSelectedEnemy(BaseEnemyUnit unit)
    {
        /// Set the selected Enemy unit
        /// Args:
        ///     BaseEnemyUnit unit: The unit to select
        Debug.Log($"Select {unit}");
        if (SelectedEnemy)
        {
            SelectedEnemy.selectionMarker.SetActive(false);
            SelectedEnemy.hideHighlight();
        }
        if (unit && unit.getRemainingActions() == 0) return;

        SelectedEnemy = unit;
        if (SelectedEnemy) {

            SelectedEnemy.selectionMarker.SetActive(true);
            SelectedEnemy.displayHighlight();
            SelectedEnemy.onSelect();
        }
    }

    public bool checkRemainingPlayerActions()
    {
        /// Check if there are any player units with remaining actions; if not, move to enemy turn
        /// Returns:
        ///     True if actions remain, false otherwise
        return checkRemainingActions(allyUnits.Cast<BaseUnit>().ToList(), GameState.EnemyTurn);
    }
    public bool checkRemainingEnemyActions()
    {
        /// Check if there are any enemy units with remaining actions; if not, move to player turn
        /// Returns:
        ///     True if actions remain, false otherwise
        return checkRemainingActions(enemyUnits.Cast<BaseUnit>().ToList(), GameState.PlayerTurn);
    }

    private bool checkRemainingActions(List<BaseUnit> units, GameState state)
    {
        /// Check if there are any actions remaining for a given faction and change state if not.
        /// Args:
        ///     List<BaseUnit> units: The unit list to check
        ///     GameState state: The state to change to
        /// Returns:
        ///     True if actions remain, false otherwise
        if (!units.Exists(u => u.getRemainingActions() > 0))
        {
            GameManager.Instance.UpdateGameState(state);
            return false;
        }
        else
        {
            return true;
        }
    }
    public void setNextEnemy()
    {
        /// Select the next Enemy Unit to take actions
        if (checkRemainingEnemyActions())
        {
            int nextEnemy = enemyUnits.FindIndex(u => u.getRemainingActions() > 0);
            SetSelectedEnemy(enemyUnits[nextEnemy]);
            SelectedEnemy.selectAction();
            CameraController.Instance.centreCamera(SelectedEnemy.OccupiedTile.get3dLocation());
        }
        else SetSelectedEnemy(null);
    }

    public void setNextPlayer(BasePlayerUnit unit = null, bool forceCamera = false)
    {
        /// Select the next Player Unit to take actions
        /// 
        Debug.Log("Select next player");
        if (checkRemainingPlayerActions())
        {
            int thisUnit = unit != null ? allyUnits.FindIndex(u => u == unit): -1;
            int index = thisUnit;
            do
            {
                index++;
                if (index >= allyUnits.Count) index = 0;
                if(allyUnits[index].getRemainingActions() > 0)
                {
                    SetSelectedHero(allyUnits[index]);
                    CameraController.Instance.centreCameraOnObject(allyUnits[index].OccupiedTile.gameObject, forceCamera);
                    break;
                }

            } while (index != thisUnit);
            GameManager.Instance.inputEnabled = true;
        }
        else SetSelectedHero(null);
    }
    public void RemoveUnit(BaseUnit unit)
    {
        /// Remove a unit from the board
        /// Args:
        ///     BaseUnit unit: The unit to remove
        Debug.Log($"Remove {unit} of faction {unit.faction}");
        unit.OccupiedTile.occupiedUnit = null;
        if (unit.faction == Faction.Player) allyUnits.Remove((BasePlayerUnit)unit);
        else if (unit.faction == Faction.Enemy) enemyUnits.Remove((BaseEnemyUnit)unit);

        if (!unit.unitTypes.Contains(UnitType.Leader))
        {
            BaseUnit leader = leaders.Find(t => t.unitIsInDetachment(unit));
            if (leader != null) leader.removeDetachmentMember(unit);
        }
        else
        {
            leaders.Remove(unit);
        }

        Destroy(unit.gameObject);
    }

    public bool checkRemainingUnits(Faction faction)
    {
        /// Check if any units remain for the other team, and move to the corresponding end screen if not.
        /// Args:
        ///     Faction faction: The current faction (if this is 'Enemy', then remaining ally units will be checked, and vice versa)
        /// Returns:
        ///     True if units remain, false otherwise
        if(faction == Faction.Enemy)
        {
            if(allyUnits.Count == 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Defeat);
                SetSelectedEnemy(null);
                return false;
            }
        }
        else if (faction == Faction.Player)
        {
            if (enemyUnits.Count == 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Victory);
                SetSelectedHero(null);
                return false;
            }
        }
        return true;
    }

    public void setUnitNumbers(
        int numberOfSpearmen = 0, 
        int numberofDemons = 0, 
        int numberofMuskets = 0,
        int numberofKites = 0,
        int numberofFieldGuns = 0,
        int numberOfTemplars = 0,
        int numberOfInfernalEngines = 0,
        int numberOfCultists = 0,
        int numberOfOrganGuns = 0,
        int numberOfHellspawn = 0)
    {
        spearmen = numberOfSpearmen;
        demons = numberofDemons;
        musketeers = numberofMuskets;
        kites = numberofKites;
        field_guns = numberofFieldGuns;
        templars = numberOfTemplars;
        infernal_engines = numberOfInfernalEngines;
        cultists = numberOfCultists;
        organ_guns = numberOfOrganGuns;
        hellspawn = numberOfHellspawn;
    }




}


public static class DetachmentData
{

    public const string SPEARMEN = "SpearmanDetachment";
    public const string MUSKETS = "MusketDetachment";
    public const string FIELD_GUNS = "FieldGunDetachment";
    public const string ORGAN_GUNS = "OrganGunDetachment";
    public const string TEMPLARS = "TemplarDetachment";

    public const string CULTISTS = "CultistDetachment";
    public const string HELLSPAWN = "HellspawnDetachment";
    public const string DEMONS = "DemonDetachment";
    public const string KITES = "KiteDetachment";
    public const string INFERNAL_ENGINES = "InfernalEngineDetachment";


    private static List<ScriptableDetachment> _detachments;
    private static List<Material> _detachmentColours = new List<Material>(Resources.LoadAll<Material>("detachmentColours"));

    public static ScriptableDetachment getDetachment(string detachmentName)
    {
        if(_detachments is null)
        {
            _detachments = new List<ScriptableDetachment>(Resources.LoadAll<ScriptableDetachment>("Detachments"));
        }

        return _detachments.Where(u => u.name == detachmentName).First();
    }
    public static List<ScriptableDetachment> getAllDetachments()
    {
        if (_detachments is null)
        {
            _detachments = new List<ScriptableDetachment>(Resources.LoadAll<ScriptableDetachment>("Detachments"));
        }

        return _detachments;
    }
    public static Material getDetachmentColour(int colour)
    {
        if(_detachmentColours is null) _detachmentColours = new List<Material>(Resources.LoadAll<Material>("detachmentColours"));
        return _detachmentColours[colour];
    }

    public static List<Material> getAllColours()
    {
        if (_detachmentColours is null) _detachmentColours = new List<Material>(Resources.LoadAll<Material>("detachmentColours"));
        return _detachmentColours;

    }


}