using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    [SerializeField] private int allies;
    [SerializeField] private int enemies;

    public BasePlayerUnit SelectedUnit;

    private List<ScriptableUnit> _units;
    void Awake()
    {
        Instance = this;
        _units = new List<ScriptableUnit>(Resources.LoadAll<ScriptableUnit>("Units"));
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
}
