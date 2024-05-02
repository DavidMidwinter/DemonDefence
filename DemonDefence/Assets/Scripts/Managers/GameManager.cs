using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

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
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

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
