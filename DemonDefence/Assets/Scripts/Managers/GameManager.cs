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
    public bool isPaused;

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        UpdateGameState(GameState.InstructionPage);
        isPaused = false;
    }
    public void UpdateGameState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.InstructionPage:
                break;
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
                StartCoroutine(exitGame());
                break;
            case GameState.Defeat:
                StartCoroutine(exitGame());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);

    }
    public IEnumerator exitGame()
    {
        StartCoroutine(PauseGame(5f));

        while (isPaused)
        {
            yield return 0;
        }

        Application.Quit();
    }
    public IEnumerator PauseGame(float pauseTime, bool halt = true)
    {
        Debug.Log("Inside PauseGame()");
        isPaused = true;
        if (halt) Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Done with my pause");
    }
}

public enum GameState
{
    InstructionPage,
    CreateGrid,
    SpawnPlayer,
    SpawnEnemy,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}
