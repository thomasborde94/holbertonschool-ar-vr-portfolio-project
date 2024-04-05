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

    //public event EventHandler OnStateChanged;
    //public event EventHandler OnGameStarting;
    public event EventHandler OnLocalPlayerReadyChanged;


    public NetworkVariable<State> state = new NetworkVariable<State>(State.BeforePlaying);
    public bool isLocalPlayerReady;

    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(1f);

    public enum State
    {
        BeforePlaying,
        WaitingToStart,
        GamePlaying,
    }

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        
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
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    //OnGameStarting?.Invoke(this, EventArgs.Empty);
                }
                break;
        }

        if (state.Value == State.WaitingToStart)
        {
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
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
}
