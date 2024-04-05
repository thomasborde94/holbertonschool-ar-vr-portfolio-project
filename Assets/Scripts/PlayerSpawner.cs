using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class PlayerSpawner : NetworkBehaviour
{
    /*
    [SerializeField] private GameObject playerPrefab;

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
    */
}
