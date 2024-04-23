using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChoosingSkillsUI : NetworkBehaviour
{
    public static ChoosingSkillsUI Instance;

    [SerializeField] public GameObject choosingSkillsUI;
    [SerializeField] public GameObject parentSKillsUI;
    [SerializeField] private Transform driverUpgradeParent;
    [SerializeField] private Transform shooterUpgradeParent;
    [SerializeField] private Button keepGoingButton;
    [SerializeField] private Button rollButton;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private GameObject[] _slots = new GameObject[0];
    [SerializeField] private GameObject[] _driverUpgrades = new GameObject[0];
    [SerializeField] private GameObject[] _shooterUpgrades = new GameObject[0];

    [SerializeField] private int _rollCost;
    [SerializeField] private TextMeshProUGUI _rollCostText;

    private Dictionary<ulong, bool> playerReadyDictionary;
    private bool assignedRole = false;
    private bool shouldIncreaseRollCost;
    private int _initialRollCost;
    private bool rollForNextRound;

    private void Awake()
    {
        Instance = this;
        shouldIncreaseRollCost = false;
        _initialRollCost = _rollCost;
        _rollCostText.text = _rollCost.ToString();
        playerReadyDictionary = new Dictionary<ulong, bool>();
        keepGoingButton.onClick.AddListener(() =>
        {
            if (TankstormGameManager.Instance.state.Value == TankstormGameManager.State.ChoosingSkills)
            {
                rollForNextRound = true;
                SetPlayerReady();
                shouldIncreaseRollCost = false;
                Roll();
                rollForNextRound = false;
                _rollCost = _initialRollCost;
                _rollCostText.text = _rollCost.ToString();
            }
        });
        rollButton.onClick.AddListener(() =>
        {
            shouldIncreaseRollCost = true;
            Roll();
        });
    }

    private void Start()
    {
        if (parentSKillsUI.activeSelf)
            parentSKillsUI.SetActive(false);
        readyText.text = "Keep Going !";
        if (_driverUpgrades.Length > 0 && _driverUpgrades[0] != null)
        {
            driverUpgradeParent = _driverUpgrades[0].transform.parent;
        }
        if (_shooterUpgrades.Length > 0 && _shooterUpgrades[0] != null)
        {
            shooterUpgradeParent = _shooterUpgrades[0].transform.parent;
        }
        else
        {
            Debug.LogWarning("No upgrades or first upgrade is null!");
        }
        
    }

    private void Update()
    {
        if (Player.Instance != null)
        {
            if (!assignedRole)
            {
                if (Player.Instance.PlayerRoleString() == "Driver")
                {
                    AssignRandomDriverUpgradeToRandomSlot();
                    Debug.Log("i am driver");
                }
                else if (Player.Instance.PlayerRoleString() == "Shooter")
                {
                    AssignRandomShooterUpgradeToRandomSlot();
                    Debug.Log("i am shooter");
                }
                assignedRole = true;
            }
        }
    }

    private void AssignRandomDriverUpgradeToRandomSlot()
    {
        List<int> availableUpgradeIndices = new List<int>();
        for (int i = 0; i < _driverUpgrades.Length; i++)
        {
            availableUpgradeIndices.Add(i);
        }

        foreach (GameObject slot in _slots)
        {
            // Vérifiez s'il reste des upgrades disponibles
            if (availableUpgradeIndices.Count == 0)
            {
                Debug.LogWarning("No upgrades available!");
                return;
            }

            // Sélectionnez un indice d'upgrade aléatoire dans la liste des indices disponibles
            int randomUpgradeIndex = UnityEngine.Random.Range(0, availableUpgradeIndices.Count);
            int selectedUpgradeIndex = availableUpgradeIndices[randomUpgradeIndex];

            // Sélectionnez l'upgrade correspondant à l'indice choisi
            GameObject selectedUpgrade = _driverUpgrades[selectedUpgradeIndex];

            // Si le slot ou l'upgrade sélectionné est null, passez au suivant
            if (slot == null || selectedUpgrade == null)
            {
                Debug.LogWarning("Slot or selected upgrade is null!");
                continue;
            }

            // Définissez le parent de l'upgrade comme le slot sélectionné
            selectedUpgrade.transform.SetParent(slot.transform, false);
            selectedUpgrade.gameObject.SetActive(true);

            // Supprimez l'indice de la liste des indices disponibles pour éviter qu'il soit choisi à nouveau
            availableUpgradeIndices.RemoveAt(randomUpgradeIndex);
        }
    }
    private void AssignRandomShooterUpgradeToRandomSlot()
    {
        List<int> availableUpgradeIndices = new List<int>();
        for (int i = 0; i < _shooterUpgrades.Length; i++)
        {
            availableUpgradeIndices.Add(i);
        }

        foreach (GameObject slot in _slots)
        {
            // Vérifiez s'il reste des upgrades disponibles
            if (availableUpgradeIndices.Count == 0)
            {
                Debug.LogWarning("No upgrades available!");
                return;
            }

            // Sélectionnez un indice d'upgrade aléatoire dans la liste des indices disponibles
            int randomUpgradeIndex = UnityEngine.Random.Range(0, availableUpgradeIndices.Count);
            int selectedUpgradeIndex = availableUpgradeIndices[randomUpgradeIndex];

            // Sélectionnez l'upgrade correspondant à l'indice choisi
            GameObject selectedUpgrade = _shooterUpgrades[selectedUpgradeIndex];

            // Si le slot ou l'upgrade sélectionné est null, passez au suivant
            if (slot == null || selectedUpgrade == null)
            {
                Debug.LogWarning("Slot or selected upgrade is null!");
                continue;
            }

            // Définissez le parent de l'upgrade comme le slot sélectionné
            selectedUpgrade.transform.SetParent(slot.transform, false);
            selectedUpgrade.gameObject.SetActive(true);

            // Supprimez l'indice de la liste des indices disponibles pour éviter qu'il soit choisi à nouveau
            availableUpgradeIndices.RemoveAt(randomUpgradeIndex);
        }
    }

    private void Roll()
    {
        if (shouldIncreaseRollCost && Player.Instance.coinAmount > _rollCost)
        {
            if (Player.Instance.PlayerRoleString() == "Driver")
            {
                ResetDriverUpgradesToInitialParent();
                AssignRandomDriverUpgradeToRandomSlot();
                ResetDriverUpdatesAvailability();
            }
            if (Player.Instance.PlayerRoleString() == "Shooter")
            {
                ResetShooterUpgradesToInitialParent();
                AssignRandomShooterUpgradeToRandomSlot();
                ResetShooterUpdatesAvailability();
            }

            {
                SpendCoins();
                UpdateRollCost();
            }
        }
        if (rollForNextRound)
        {
            if (Player.Instance.PlayerRoleString() == "Driver")
            {
                ResetDriverUpgradesToInitialParent();
                AssignRandomDriverUpgradeToRandomSlot();
                ResetDriverUpdatesAvailability();
            }
            if (Player.Instance.PlayerRoleString() == "Shooter")
            {
                ResetShooterUpgradesToInitialParent();
                AssignRandomShooterUpgradeToRandomSlot();
                ResetShooterUpdatesAvailability();
            }
        }
        else
            Debug.Log("not enough money to roll");
    }

    public void ResetDriverUpdatesAvailability()
    {
        foreach(GameObject driverUpgrade in _driverUpgrades)
        {
            UpgradeSingle upgrade = driverUpgrade.GetComponent<UpgradeSingle>();
            upgrade.isAvailable = true;

            Color currentColor = upgrade.GetComponent<Image>().color;
            currentColor.a = 1;
            upgrade.GetComponent<Image>().color = currentColor;
        } 
    }
    public void ResetShooterUpdatesAvailability()
    {
        foreach (GameObject driverUpgrade in _shooterUpgrades)
        {
            UpgradeSingle upgrade = driverUpgrade.GetComponent<UpgradeSingle>();
            upgrade.isAvailable = true;

            Color currentColor = upgrade.GetComponent<Image>().color;
            currentColor.a = 1;
            upgrade.GetComponent<Image>().color = currentColor;
        }
    }
    public void ResetDriverUpgradesToInitialParent()
    {
        foreach (GameObject upgrade in _driverUpgrades)
        {
            if (upgrade != null)
            {
                upgrade.transform.SetParent(driverUpgradeParent, false);
                upgrade.gameObject.SetActive(false);
            }
        }
    }
    public void ResetShooterUpgradesToInitialParent()
    {
        foreach (GameObject upgrade in _shooterUpgrades)
        {
            if (upgrade != null)
            {
                upgrade.transform.SetParent(shooterUpgradeParent, false);
                upgrade.gameObject.SetActive(false);
            }
        }
    }

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
                Debug.Log("Not all clients are ready");
                break;
            }
        }

        if (allClientsReady)
        {
            HideChoosingSkillsClientRpc();
            Debug.Log("All clients are ready");
            TankstormGameManager.Instance.state.Value = TankstormGameManager.State.GamePlaying;
        }

    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
    }

    [ClientRpc]
    public void HideChoosingSkillsClientRpc()
    {
        parentSKillsUI.SetActive(false);
    }

    [ClientRpc]
    public void ShowChoosingSkillsClientRpc()
    {
        parentSKillsUI.SetActive(true);
    }

    public void SetPlayerReady()
    {
        readyText.text = "Waiting for the other player";
        SetPlayerReadyServerRpc();
    }

    [ClientRpc]
    public void ResetMenuClientRpc()
    {
        readyText.text = "Keep Going !";
        parentSKillsUI.SetActive(false);
    }

    [ServerRpc(RequireOwnership =false)]
    public void ResetReadyDictionnaryServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerNotReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = false;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            playerReadyDictionary[clientId] = false;
        }
    }

    [ClientRpc]
    private void SetPlayerNotReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = false;
    }

    private void UpdateRollCost()
    {
        _rollCost += (int)(_rollCost * (50.0f / 100.0f));
        _rollCostText.text = _rollCost.ToString();
    }

    private void SpendCoins()
    {
        Player.Instance.coinAmount -= _rollCost;
        CoinsUI.Instance.UpdateCoinAmout();
    }
}