using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankstormGameManager : NetworkBehaviour
{
    public static TankstormGameManager Instance { get; private set; }

    public event EventHandler OnLocalPlayerReadyChanged;

    public NetworkVariable<State> state = new NetworkVariable<State>(State.BeforePlaying);
    public bool isLocalPlayerReady;
    public NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    public float gamePlayingTimerMax = 15f;
    public bool justStartedRound = true;
    private bool justAddedRound = false;
    private GameObject _wolfBoss;
    private bool finishedGame;
    [HideInInspector] public bool playerLost;
    [HideInInspector] public bool playerWon;

    private bool playedPlayerWonSound = false;

    public enum State
    {
        BeforePlaying,
        WaitingToStart,
        GamePlaying,
        ChoosingSkills,
    }

    private Dictionary<ulong, bool> playerReadyDictionary;


    #region Unity Lifecycle
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerReadyDictionary = new Dictionary<ulong, bool>();

    }

    private void Start()
    {
        finishedGame = false;
        playerWon = false;
        playerLost = false;
        playedPlayerWonSound = false;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        switch (state.Value)
        {
            case State.BeforePlaying:
                break;
            case State.WaitingToStart:
                break;
            case State.GamePlaying:
                if (EnemySpawner.Instance != null)
                {
                    if (justStartedRound && EnemySpawner.Instance.currentRound != 5)
                        BeginningOfRound();

                    gamePlayingTimer.Value -= Time.deltaTime;

                    if (gamePlayingTimer.Value <= 0f && EnemySpawner.Instance.currentRound != 5)
                    {
                        Player.Instance._currentHealth.Value = Player.Instance._maxHealth;
                        state.Value = State.ChoosingSkills;
                    }
                    if (EnemySpawner.Instance.currentRound == 5)
                        BeginningOfRoundFive();
                }
                
                break;
            case State.ChoosingSkills:
                justStartedRound = true;
                if (!justAddedRound)
                {
                    EnemySpawner.Instance.KillAllEnemiesServerRpc();
                    if (!playerLost)
                    {
                        ChoosingSkillsUI.Instance.ShowChoosingSkillsClientRpc();
                    }
                    EnemySpawner.Instance.shouldSpawn = false;
                    EnemySpawner.Instance.currentRound += 1;
                    justAddedRound = true;
                }
                break;
        }

        if (state.Value == State.WaitingToStart)
        {
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    private void BeginningOfRound()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (ChoosingSkillsUI.Instance != null)
                ChoosingSkillsUI.Instance.ResetMenuClientRpc();
            else
                Debug.Log("cant find instance");
        }

        ChoosingSkillsUI.Instance.ResetReadyDictionnaryServerRpc();
        EnemySpawner.Instance._timeBetweenSpawn = EnemySpawner.Instance._timeBetweenSpawnRank * (1.5f / EnemySpawner.Instance.currentRound);
        EnemySpawner.Instance.shouldSpawn = true;
        justAddedRound = false;
        gamePlayingTimer.Value = gamePlayingTimerMax;
        justStartedRound = false;
    }

    private void BeginningOfRoundFive()
    {
        if (!finishedGame)
        {
            EnemySpawner.Instance._timeBetweenSpawn = EnemySpawner.Instance._timeBetweenSpawnRank * (1.5f / EnemySpawner.Instance.currentRound);
            EnemySpawner.Instance.shouldSpawn = true;
        }
        GameClockUI.Instance.Hide();
        Debug.Log("in round 5");
        _wolfBoss = GameObject.Find("WolfBoss(Clone)");
        if (_wolfBoss == null)
            Debug.Log("_wolfboss is null");
        else
        {
            if (_wolfBoss.GetComponent<Enemy>().isDead)
            {
                if (!playedPlayerWonSound)
                {
                    SFXManager.Instance.PlaySFX(11);
                    playedPlayerWonSound = true;
                }
                Round5UI.Instance.ShowParentClientRpc();
                EnemySpawner.Instance.shouldSpawn = false;
                EnemySpawner.Instance.KillAllEnemiesServerRpc();
                justStartedRound = true;
                finishedGame = true;
                playerWon = true;
            }
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Sets value of the clientID Key to true
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // The player is not ready
                allClientsReady = false;
                state.Value = State.WaitingToStart;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.GamePlaying;
        }

        Debug.Log("All clients ready:  " + allClientsReady);
    }

    #region Public properties

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }
    public bool IsBeforePlaying()
    {
        return state.Value == State.BeforePlaying;
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    #endregion
}
