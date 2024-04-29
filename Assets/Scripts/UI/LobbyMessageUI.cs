using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(HideAndPlaySFX);
    }

    private void Start()
    {
        TankstormGameMultiplayer.Instance.OnFailedToJoinGame += TankstormGameMultiplayer_OnFailedToJoinGame;
        TankstormLobby.Instance.OnCreateLobbyStarted += TankstormLobby_OnCreateLobbyStarted;
        TankstormLobby.Instance.OnCreateLobbyFailed += TankstormLobby_OnCreateLobbyFailed;
        TankstormLobby.Instance.OnJoinStarted += TankstormLobby_OnJoinStarted;
        TankstormLobby.Instance.OnJoinFailed += TankstormLobby_OnJoinFailed;
        TankstormLobby.Instance.OnQuickJoinFailed += TankstormLobby_OnQuickJoinFailed;
        Hide();
    }

    private void TankstormLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Could not find a lobby to Quick Join");
    }

    private void TankstormLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join lobby...");
    }

    private void TankstormLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining lobby...");
    }

    private void TankstormLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to create lobby");
    }

    private void TankstormLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void TankstormGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HideAndPlaySFX()
    {
        SFXManager.Instance.PlaySFX(0);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        TankstormGameMultiplayer.Instance.OnFailedToJoinGame -= TankstormGameMultiplayer_OnFailedToJoinGame;
        TankstormLobby.Instance.OnCreateLobbyStarted -= TankstormLobby_OnCreateLobbyStarted;
        TankstormLobby.Instance.OnCreateLobbyFailed -= TankstormLobby_OnCreateLobbyFailed;
        TankstormLobby.Instance.OnJoinStarted -= TankstormLobby_OnJoinStarted;
        TankstormLobby.Instance.OnJoinFailed -= TankstormLobby_OnJoinFailed;
        TankstormLobby.Instance.OnQuickJoinFailed -= TankstormLobby_OnQuickJoinFailed;
    }
}
