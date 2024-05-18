using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    [SerializeField] private int allies;
    [SerializeField] private int enemies;

    public List<BasePlayerUnit> allyUnits;
    public List<BaseEnemy> enemyUnits;


    public BasePlayerUnit SelectedUnit;
    public BaseEnemy SelectedEnemy;

    private List<ScriptableUnit> _units;

    void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerStateChanged;
        Instance = this;
        _units = new List<ScriptableUnit>(Resources.LoadAll<ScriptableUnit>("Units"));
        allyUnits = new List<BasePlayerUnit>();
        enemyUnits = new List<BaseEnemy>();
    }

    private void GameManagerStateChanged(GameState state)
    {
        if (state == GameState.EnemyTurn)
        {
            foreach (BaseEnemy u in enemyUnits)
            {
                u.setRemainingActions(u.maxActions);
            }
            if (enemyUnits.Count > 0)
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
        for (int i = 0; i < allies; i++)
        {
            var randomPrefab = GetRandomUnit<BaseUnit>(Faction.Player);
            var spawnedUnit = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();
            spawnedUnit.transform.position = randomSpawnTile.transform.position;

            randomSpawnTile.SetUnit(spawnedUnit);
            spawnedUnit.setRemainingActions(spawnedUnit.maxActions);
            allyUnits.Add((BasePlayerUnit)spawnedUnit);
        }
        GameManager.Instance.UpdateGameState(GameState.SpawnEnemy);
    }
    public void spawnEnemy()
    {
        for (int i = 0; i < enemies; i++)
        {
            var randomPrefab = GetRandomUnit<BaseUnit>(Faction.Enemy);
            var spawnedUnit = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            spawnedUnit.transform.position = randomSpawnTile.transform.position;

            randomSpawnTile.SetUnit(spawnedUnit);
            enemyUnits.Add((BaseEnemy)spawnedUnit);
        }
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().unitPrefab;
    }

    public void SetSelectedHero(BasePlayerUnit unit)
    {
        Debug.Log($"Select {unit}");
        if (unit && unit.getRemainingActions() == 0) return;

        if (SelectedUnit) SelectedUnit.selectionMarker.SetActive(false);
        SelectedUnit = unit;
        if (SelectedUnit)
        {
            SelectedUnit.calculateAllTilesInRange();
            SelectedUnit.selectionMarker.SetActive(true);
        }

    }

    public void SetSelectedEnemy(BaseEnemy unit)
    {
        Debug.Log($"Select {unit}");
        if (unit && unit.getRemainingActions() == 0) return;

        SelectedEnemy = unit;

    }

    public void takeAction()
    {
        SelectedUnit.takeAction();

    }

    public bool checkRemainingPlayerActions()
    {
        if(!allyUnits.Exists(u => u.getRemainingActions() > 0))
        {
            GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool checkRemainingEnemyActions()
    {
        if (!enemyUnits.Exists(u => u.getRemainingActions() > 0))
        {
            GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void setNextEnemy()
    {
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
        Debug.Log($"Remove {unit} of faction {unit.faction}");
        if (unit.faction == Faction.Player) allyUnits.Remove((BasePlayerUnit)unit);
        else if (unit.faction == Faction.Enemy) enemyUnits.Remove((BaseEnemy)unit);
    }

}
