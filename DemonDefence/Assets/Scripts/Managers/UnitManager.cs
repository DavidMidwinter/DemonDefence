using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public BasePlayerUnit SelectedUnit;

    private List<ScriptableUnit> _units;
    void Awake()
    {
        Instance = this;
        _units = new List<ScriptableUnit>(Resources.LoadAll<ScriptableUnit>("Units"));
    }

    public void spawnPlayer()
    {
        var playerCount = 5;
        for (int i = 0; i < playerCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseUnit>(Faction.Player);
            var spawnedUnit = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();

            randomSpawnTile.SetUnit(spawnedUnit);
        }
        GameManager.Instance.UpdateGameState(GameState.SpawnEnemy);
    }
    public void spawnEnemy()
    {
        var enemyCount = 5;
        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseUnit>(Faction.Enemy);
            var spawnedUnit = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

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
        if (SelectedUnit) SelectedUnit.selectionMarker.SetActive(false);
        SelectedUnit = unit;
        if (SelectedUnit)
        {
            unit.calculateAllTilesInRange();
            SelectedUnit.selectionMarker.SetActive(true);
        }

    }
}
