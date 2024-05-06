using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameobject;
    [SerializeField] private PlayerRole playerRole;
    [SerializeField] private TextMeshPro playerNameText;



    private void Start()
    {
        TankstormGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += TankstormGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;


        UpdatePlayer();
    }


    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void TankstormGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    // Updates playerRole when selecting role
    private void UpdatePlayer()
    {
        if (TankstormGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            // Shows the player on screen if connected
            Show();

            PlayerData playerData = TankstormGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            playerRole.SetPlayerRole(TankstormGameMultiplayer.Instance.GetPlayerRole(playerData.roleId));

            if (playerData.roleId != 2)
            {
                readyGameobject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            }

            playerNameText.text = playerData.playerName.ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        TankstormGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= TankstormGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
