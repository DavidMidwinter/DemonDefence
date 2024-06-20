using System.Collections;
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
    [SerializeField] private int sergeants;


    [SerializeField] private int enemies;

    public List<BasePlayerUnit> allyUnits;
    public List<BaseEnemyUnit> enemyUnits;


    public BasePlayerUnit SelectedUnit;
    public BaseEnemyUnit SelectedEnemy;

    private List<ScriptableUnit> _units;
    private List<ScriptableDetachment> _detachments;

    void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerStateChanged;
        Instance = this;
        _units = new List<ScriptableUnit>(Resources.LoadAll<ScriptableUnit>("Units"));
        _detachments = new List<ScriptableDetachment>(Resources.LoadAll<ScriptableDetachment>("Detachments"));
        allyUnits = new List<BasePlayerUnit>();
        enemyUnits = new List<BaseEnemyUnit>();
    }

    private void GameManagerStateChanged(GameState state)
    {
        /// When the GameState changes, this method is called. 
        /// If the state is the player turn, set all player unit remaining actions to their default
        /// If the state is the enemy turn, set all enemy unit remaining actions to their default, then select the first enemy unit
        if (state == GameState.EnemyTurn)
        {
            foreach (BaseEnemyUnit u in enemyUnits)
            {
                u.setRemainingActions(u.maxActions);
            }
            if (checkRemainingEnemyActions())
                setNextEnemy();
        }
        if(state == GameState.PlayerTurn)
        {
            foreach(BasePlayerUnit u in allyUnits)
            {
                u.setRemainingActions(u.maxActions);
            }
        }
    }



    public void spawnPlayer()
    {
        /// Spawn a Player Unit on a random Spawn Tile
        /*for (int i = 0; i < spearmen; i++)
        {
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();
            if (randomSpawnTile == null) break;
            var randomPrefab = GetUnitPrefab<BaseUnit>(Faction.Player, "Spearman");
            spawnUnit(randomPrefab, randomSpawnTile);
        }

        for (int i = 0; i < sergeants; i++)
        {
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();
            if (randomSpawnTile == null) break;
            var randomPrefab = GetUnitPrefab<BaseUnit>(Faction.Player, "Sergeant");
            var spawnedUnit = Instantiate(randomPrefab);
            spawnedUnit.transform.position = randomSpawnTile.transform.position;

            randomSpawnTile.SetUnit(spawnedUnit);
            spawnedUnit.setRemainingActions(spawnedUnit.maxActions);
            allyUnits.Add((BasePlayerUnit)spawnedUnit);
        }*/

        ScriptableDetachment detachment = _detachments.Where(u => u.Faction == Faction.Player && u.name == DetachmentData.SPEARMEN).First();
        spawnDetachment(detachment);
        GameManager.Instance.UpdateGameState(GameState.SpawnEnemy);
    }

    public void spawnDetachment(ScriptableDetachment detachment)
    {
        /// Spawn a detachment in the player spawn zone
        /// Args:
        ///     ScriptableDetachment detachment: the detachment to spawn
        Tile randomSpawnTile;
        if (detachment.leaderUnit)
        {
            randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();
            if (randomSpawnTile == null) return;
            spawnUnit(detachment.leaderUnit, randomSpawnTile);
        }
        for (int i = 0; i < detachment.numberOfTroops; i++)
        {
            randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();
            if (randomSpawnTile == null) break;
            spawnUnit(detachment.troopUnit, randomSpawnTile);
        }
    }
    public void spawnHostileDetachment(ScriptableDetachment detachment)
    {
        /// Spawn a detachment in the enemy spawn zone
        /// Args:
        ///     ScriptableDetachment detachment: the detachment to spawn
        Tile randomSpawnTile;
        if (detachment.leaderUnit)
        {
            randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            if (randomSpawnTile == null) return;
            spawnUnit(detachment.leaderUnit, randomSpawnTile);
        }
        for (int i = 0; i < detachment.numberOfTroops; i++)
        {
            randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            if (randomSpawnTile == null) break;
            spawnUnit(detachment.troopUnit, randomSpawnTile);
        }
    }
    public void spawnUnit(BaseUnit unit, Tile tile)
    {
        /// Spawn a unit on a given tile, and add unit to their corresponding list
        /// Args:
        ///     BaseUnit unit: THe unit to spawn
        ///     Tile tile: The tile to spawn them on
        var spawnedUnit = Instantiate(unit);
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
    }
    public void spawnEnemy()
    {
        /// Spawn an Enemy Unit on a random Spawn Tile
        /*for (int i = 0; i < enemies; i++)
        {
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            if (randomSpawnTile == null) break;
            var randomPrefab = GetRandomUnitPrefab<BaseUnit>(Faction.Enemy);
            spawnUnit(randomPrefab, randomSpawnTile);
        }*/

        ScriptableDetachment detachment = _detachments.Where(u => u.Faction == Faction.Enemy && u.name == DetachmentData.DEMONS).First();
        spawnHostileDetachment(detachment);

        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
    }

    private T GetRandomUnitPrefab<T>(Faction faction) where T : BaseUnit
    {
        /// Gets a random Unit Prefab for a given Faction
        /// Args:
        ///     Faction faction: The faction to get a unit for
        ///     T: The unit object type, so that specific child object types can be used
        /// Returns:
        ///     A random unit prefab, type T.
        ///     
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().unitPrefab;
    }
    private T GetUnitPrefab<T>(Faction faction, string name) where T: BaseUnit
    {
        /// Gets a Unit Prefab of a given Name for a given Faction
        /// Args:
        ///     Faction faction: The faction to get a unit for
        ///     string name: The name of the unit to grab
        ///     T: The unit object type, so that specific child object types can be used
        /// Returns:
        ///     A specified unit prefab, type T.
        return (T)_units.Where(u => u.Faction == faction && u.name == name).First().unitPrefab;
    }

    public void SetSelectedHero(BasePlayerUnit unit)
    {
        /// Set the selected Player unit
        /// Args:
        ///     BasePlayerUnit unit: The unit to select
        Debug.Log($"Select {unit}");
        if (unit && unit.getRemainingActions() == 0) return;
        TacticalUI.Instance.clearActions();
        if (SelectedUnit) SelectedUnit.selectionMarker.SetActive(false);
        SelectedUnit = unit;
        if (SelectedUnit)
        {
            SelectedUnit.onSelect();
            SelectedUnit.calculateAllTilesInRange();
            SelectedUnit.getAttackTargets();
            SelectedUnit.selectionMarker.SetActive(true);
        }

    }

    public void SetSelectedEnemy(BaseEnemyUnit unit)
    {
        /// Set the selected Enemy unit
        /// Args:
        ///     BaseEnemyUnit unit: The unit to select
        Debug.Log($"Select {unit}");
        if (SelectedEnemy) SelectedEnemy.selectionMarker.SetActive(false);
        if (unit && unit.getRemainingActions() == 0) return;

        SelectedEnemy = unit;
        if (SelectedEnemy) {

            SelectedEnemy.onSelect();
            SelectedEnemy.selectionMarker.SetActive(true);
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
        }
        else SetSelectedEnemy(null);
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

}


public static class DetachmentData
{
    private static Vector2 leader_coords = new Vector2(0, 0);

    public static List<Vector2> unit_coords = new List<Vector2>
    {
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(-1, 1),
        new Vector2(1, 1),
        new Vector2(-1, -1),
        new Vector2(1, -1)
    };

    public static Vector2 leader
    {
        get { return leader_coords; }
    }
    public static List<Vector2> units
    {
        get { return unit_coords; }
    }

    public const string SPEARMEN = "SpearmanDetachment";
    public const string DEMONS = "DemonDetachment";

}