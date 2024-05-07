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

    private List<ScriptableUnit> _units;

    private int waitTime = 0;

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

    public void takeAction()
    {
        SelectedUnit.takeAction();
        if(SelectedUnit.getRemainingActions() <= 0)
        {
            SetSelectedHero(null);
            return;
        }
        SelectedUnit.calculateAllTilesInRange();

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

    private void FixedUpdate()
    {
        if (GameManager.Instance.State == GameState.EnemyTurn)
        {
            waitTime += 1;
            if (waitTime >= 5 / Time.fixedDeltaTime)
            {
                GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
                waitTime = 0;
            }
        }
    }
}
