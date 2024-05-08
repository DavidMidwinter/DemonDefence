using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;

    public GameState State;

    public bool inputEnabled;

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        UpdateGameState(GameState.CreateGrid);
    }
    public void UpdateGameState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.CreateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnPlayer:
                UnitManager.Instance.spawnPlayer();
                break;
            case GameState.SpawnEnemy:
                UnitManager.Instance.spawnEnemy();
                break;
            case GameState.PlayerTurn:
                Debug.Log("Player Turn");
                inputEnabled = true;
                break;
            case GameState.EnemyTurn:
                Debug.Log("Enemy Turn");
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);

    }
}

public enum GameState
{
    CreateGrid,
    SpawnPlayer,
    SpawnEnemy,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}
