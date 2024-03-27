using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankstormGameMultiplayer : NetworkBehaviour
{

    public const int MAX_PLAYER_AMOUNT = 2;

    public static TankstormGameMultiplayer Instance;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private List<int> playerRoleList;
    [SerializeField] private GameObject playerPrefab;

    private NetworkList<PlayerData> playerDataNetworkList;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    // When PlayerDataNetworkList changes
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += PlayerSpawn_OnLoadEventCompleted;
    }

    private void PlayerSpawn_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (IsServer && sceneName == "GameScene")
        {

            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerRoleList.Count; i++) 
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // player disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            roleId = GetFirstUnusedRoleId(),
        });
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "game has already started";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
                return i;
        }
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
                return playerData;
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public int GetPlayerRole(int roleId)
    {
        return playerRoleList[roleId];
    }

    public void ChangePlayerRole(int roleId)
    {
        ChangePlayerRoleServerRpc(roleId);
    }

    [ServerRpc(RequireOwnership =false)]
    private void ChangePlayerRoleServerRpc(int roleId, ServerRpcParams serverRpcparams = default)
    {
        if (!IsRoleAvailable(roleId))
        {
            // Role not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcparams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.roleId = roleId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsRoleAvailable(int roleId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.roleId == roleId)
            {
                // role already used
                return false;
            }
        }
        return true;
    }
    

    private int GetFirstUnusedRoleId()
    {
        for (int i = 0; i < playerRoleList.Count; i++)
        {
            if (IsRoleAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public bool EveryPlayerHaveRole()
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if ((playerData.roleId == 2))
            {
                // a player does not have a role
                return false;
            }    
        }
        return true;
    }

}
