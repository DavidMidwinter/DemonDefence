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

    public delegate void notifyTiles();
    public static event notifyTiles UpdateTiles;
    public static event notifyTiles ClearTiles;
    public bool debugMode;
    public bool cameraCentring;
    public int strengthPenalty = 2;

    private GridManager gridManager => GridManager.Instance;
    private UnitManager unitManager => UnitManager.Instance;


    public GameState State;

    public bool inputEnabled;
    public bool isPaused;
    public int seed;

    private void Awake()
    {
        Instance = this;
        if(seed > 0)
            UnityEngine.Random.InitState(seed);
    }
    public void Start()
    {
        UpdateGameState(GameState.InitGame);
        isPaused = false;
    }
    
    public void initGameSettings()
    {
        gridManager.setGridSize(TacticalStartData._gridSize);
        gridManager.setCitySize(TacticalStartData._citySize);
        gridManager.setMaxBuildings(TacticalStartData._maxBuildings);
        gridManager.setSpawnRadius(TacticalStartData._spawnRadius);
        gridManager.setFileName(TacticalStartData._fileName);
        gridManager.setWalled(TacticalStartData._walled);
        gridManager.setFoliageChances(
            TacticalStartData._treeChance, 
            TacticalStartData._bushChance);
        unitManager.setUnitNumbers(
            TacticalStartData._spearmen, 
            TacticalStartData._demons, 
            TacticalStartData._muskets, 
            TacticalStartData._kites);
    }
    public void UpdateGameState(GameState newState)
    {
        /// Transition the Game State and call any functionality for that state
        /// Args:
        ///     GameState newState: The state to transition to.
        State = newState;
        switch (newState)
        {
            case GameState.InitGame:
                initGameSettings();
                UpdateGameState(GameState.CreateGrid);
                return;
            case GameState.CreateGrid:
                gridManager.GenerateGrid();
                break;
            case GameState.SpawnPlayer:
                unitManager.spawnPlayer();
                break;
            case GameState.SpawnEnemy:
                unitManager.spawnEnemy();
                break;
            case GameState.PlayerTurn:
                Debug.Log("Player Turn");
                inputEnabled = true;
                break;
            case GameState.EnemyTurn:
                Debug.Log("Enemy Turn");
                inputEnabled = false;
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
        StartCoroutine(PauseGame(5f, false));

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

    public void updateTiles()
    {
        Debug.Log(inputEnabled);
        UpdateTiles.Invoke();
    }

    public void clearTiles()
    {
        Debug.Log("Clear all tiles");
        ClearTiles.Invoke();

    }
}

public enum GameState
{
    InstructionPage,
    InitGame,
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