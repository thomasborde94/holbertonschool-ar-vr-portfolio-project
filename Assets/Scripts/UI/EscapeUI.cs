using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class EscapeUI : NetworkBehaviour
{
    [SerializeField] private GameObject escapeUI;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioMixer audioMixer;

    private bool escapeUIShowing;

    private void Awake()
    {
        escapeUI.SetActive(false);
        playAgainButton.onClick.AddListener(() =>
        {

            TankstormGameMultiplayer.Instance.InitializeNetworkVariables();
            TankstormGameManager.Instance.playerLost = false;
            HideParentServerRpc();
            _loadingUI.SetActive(true);
            Loader.ReloadScene();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            LoadMainMenuServerRpc();

        });
    }

    private void Start()
    {
        HideParentServerRpc();
        if (!IsServer)
        {
            playAgainButton.gameObject.SetActive(false);
        }
        else
        {
            playAgainButton.gameObject.SetActive(true);
        }
    }
    void Update()
    {
        HandleEscapeUI();
        //SetMasterVolume();
    }

    private void HandleEscapeUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!escapeUIShowing)
            {
                escapeUI.SetActive(true);
                escapeUIShowing = true;
                Debug.Log("should display escapeui");
            }
            else
            {
                escapeUI.SetActive(false);
                escapeUIShowing = false;
                Debug.Log("should hide escapeui");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadMainMenuServerRpc()
    {
        TankstormGameManager.Instance.state.Value = TankstormGameManager.State.BeforePlaying;
        _loadingUI.SetActive(true);
        Loader.LoadNetwork(Loader.Scene.MainMenuScene);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HideParentServerRpc()
    {
        escapeUI.SetActive(false);
    }
}
