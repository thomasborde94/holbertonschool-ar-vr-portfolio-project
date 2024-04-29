using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;


    private void Awake()
    {
        Instance = this;
        mainMenuButton.onClick.AddListener(() =>
        {

            StartCoroutine(SFXManager.Instance.PlaySoundAndLoadMainMenuScene());
            TankstormLobby.Instance.LeaveLobby();
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            SFXManager.Instance.PlaySFX(0);
            lobbyCreateUI.Show();
            Hide();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            SFXManager.Instance.PlaySFX(0);
            TankstormLobby.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            SFXManager.Instance.PlaySFX(0);
            TankstormLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = TankstormGameMultiplayer.Instance.GetPlayerName();

        // Update playername when changed
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            TankstormGameMultiplayer.Instance.SetPlayerName(newText);
        });

        UpdateLobbyList(new List<Lobby>());

        TankstormLobby.Instance.OnLobbyListChanged += TankstormLobby_OnLobbyListChanged;
    }

    private void TankstormLobby_OnLobbyListChanged(object sender, TankstormLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    // updates lobby list visual UI
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        TankstormLobby.Instance.OnLobbyListChanged -= TankstormLobby_OnLobbyListChanged;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
