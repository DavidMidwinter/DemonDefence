using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Functionality to control the flow of the tactical game
    /// </summary>
    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;

    public GameState State;

    public bool inputEnabled;
    public bool isPaused;
    public bool debugMode;
    public int seed;

    private void Awake()
    {
        Instance = this;
        if(seed > 0)
            UnityEngine.Random.InitState(seed);
    }
    public void Start()
    {
        UpdateGameState(GameState.InstructionPage);
        isPaused = false;
    }
    public void UpdateGameState(GameState newState)
    {
        /// Transition the Game State and call any functionality for that state
        /// Args:
        ///     GameState newState: The state to transition to.
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
        /// Exit the game. This will exit with a 5 second delay as it is called when a game end state is reached, so the user can read the result.
        StartCoroutine(PauseGame(5f));

        while (isPaused)
        {
            yield return 0;
        }

        Application.Quit();
    }
    public IEnumerator PauseGame(float pauseTime, bool halt = true)
    {
        /// Pause the game for a given period. Can either just pause game logic, or pause game time too.
        /// Args:
        ///     float pauseTime: The time to pause for
        ///     bool halt: Whether the pause should also halt the procession of game time, default true.
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
public enum Faction
{
    Player = 0,
    Enemy = 1
}