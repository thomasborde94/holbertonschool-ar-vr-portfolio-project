using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        TankstormGameManager.Instance.isLocalPlayerReady = true;
        SetPlayerReadyServerRpc();
    }


    // Sets player to Ready, if all client are ready, launchs the game Scene
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Sets value of the clientID Key to true
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // The player is not ready
                allClientsReady = false;
                TankstormGameManager.Instance.state.Value = TankstormGameManager.State.WaitingToStart;
                break;
            }
        }

        if (allClientsReady)
        {
            if (TankstormGameMultiplayer.Instance.EveryPlayerHaveRole())
            {
                TankstormLobby.Instance.DeleteLobby();
                TankstormGameManager.Instance.state.Value = TankstormGameManager.State.GamePlaying;
                Loader.LoadNetwork(Loader.Scene.GameScene);
            }
        }

    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
