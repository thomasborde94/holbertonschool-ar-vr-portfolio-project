using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Round5UI : NetworkBehaviour
{
    public static Round5UI Instance { get; private set; }


    [SerializeField] private GameObject parentGo;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _loadingUI;

    [SerializeField] private GameObject escapeUI;
    

    private bool playedLostSound = false;
    private void Awake()
    {
        Instance = this;
        _loadingUI.SetActive(false);
        

        playAgainButton.onClick.AddListener(() =>
        {

            TankstormGameMultiplayer.Instance.InitializeNetworkVariables();
            HideParentServerRpc();
            _loadingUI.SetActive(true);
            TankstormGameManager.Instance.playerLost = false;
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
        playedLostSound = false;
        if (!IsServer)
        {
            playAgainButton.gameObject.SetActive(false);
            Debug.Log("I am not server");
        }
        else
        {
            playAgainButton.gameObject.SetActive(true);
            Debug.Log("I am server");
        }
    }


    private void Update()
    {


        if (TankstormGameManager.Instance.playerWon)
            _text.text = "YOU WON !";
        if (Player.Instance != null)
        {
            if (Player.Instance._currentHealth.Value <= 0)
            {
                if (!playedLostSound)
                {
                    SFXManager.Instance.PlaySFX(9);
                    playedLostSound = true;
                }
                TankstormGameManager.Instance.playerLost = true;
                _text.text = "YOU LOST !";
                ShowParent();
                if (escapeUI.activeSelf)
                    escapeUI.SetActive(false);
                if (EnemySpawner.Instance != null)
                {
                    EnemySpawner.Instance.shouldSpawn = false;
                    if (IsServer)
                        EnemySpawner.Instance.KillAllEnemiesServerRpc();
                }
            }
        }
        
    }

    [ServerRpc(RequireOwnership =false)]
    private void LoadMainMenuServerRpc()
    {
        TankstormGameManager.Instance.state.Value = TankstormGameManager.State.BeforePlaying;
        _loadingUI.SetActive(true);
        Loader.LoadNetwork(Loader.Scene.MainMenuScene);
    }

    public void ShowParent()
    {
        parentGo.SetActive(true);
    }

    [ServerRpc(RequireOwnership =false)]
    public void HideParentServerRpc()
    {
        parentGo.SetActive(false);
    }
}
