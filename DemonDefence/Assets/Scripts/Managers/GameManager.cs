using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] public bool nightGivesEnemyTurn;
    public bool allowDiagonalMovement;
    public bool cameraCentring;
    [SerializeField] public bool isNight;
    public int strengthPenalty = 2;

    private GridManager gridManager => GridManager.Instance;
    private UnitManager unitManager => UnitManager.Instance;


    public GameState State;

    public bool inputEnabled;
    public bool delayingProcess;
    public int seed;

    public bool canInput => inputEnabled && !PauseMenu.GameIsPaused;

    private void Awake()
    {
        Instance = this;
        if(seed > 0)
            UnityEngine.Random.InitState(seed);
    }
    public void Start()
    {
        Time.timeScale = 1f;
        UpdateGameState(GameState.InitGame);
        delayingProcess = false;
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
        gridManager.setRivers(TacticalStartData._rivers);
        setIsNight(TacticalStartData._isNight);
        gridManager.setIsCity(TacticalStartData._isCity);
        gridManager.setNumberOfSpawns(TacticalStartData._playerSpawnNumber, TacticalStartData._enemySpawnNumber);


        unitManager.setUnitNumbers(
            TacticalStartData._spearmen, 
            TacticalStartData._demons, 
            TacticalStartData._muskets, 
            TacticalStartData._kites,
            TacticalStartData._field_guns,
            TacticalStartData._templars,
            TacticalStartData._infernal_engines,
            TacticalStartData._cultists);
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
                StartCoroutine(returnToMainMenu());
                break;
            case GameState.Defeat:
                StartCoroutine(returnToMainMenu());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
        OnGameStateChanged?.Invoke(newState);

    }

    public void startGame()
    {
        if (nightGivesEnemyTurn && isNight) UpdateGameState(GameState.EnemyTurn);
        else UpdateGameState(GameState.PlayerTurn);
    }

    public IEnumerator returnToMainMenu()
    {
        StartCoroutine(DelayGame(5f));

        while (delayingProcess)
        {
            yield return 0;
        }

        Utils.returnToMainMenu();
    }
    public IEnumerator exitGame()
    {
        /// Exit the game. This will exit with a 5 second delay as it is called when a game end state is reached, so the user can read the result.
        StartCoroutine(DelayGame(5f));

        while (delayingProcess)
        {
            yield return 0;
        }

        Application.Quit();
    }
    public IEnumerator DelayGame(float pauseTime)
    {
        /// Pause the game for a given period. Can either just pause game logic, or pause game time too.
        /// Args:
        ///     float pauseTime: The time to pause for
        ///     bool halt: Whether the pause should also halt the procession of game time, default true.
        Debug.Log("Inside DelayGame()");
        delayingProcess = true;
        float pauseEndTime = Time.time + pauseTime;
        while (Time.time < pauseEndTime)
        {
            yield return 0;
        }
        delayingProcess = false;
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
    public void setIsNight(bool toggle)
    {
        isNight = toggle;
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