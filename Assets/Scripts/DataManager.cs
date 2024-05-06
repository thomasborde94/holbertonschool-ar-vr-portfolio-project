using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : NetworkBehaviour
{
    public static DataManager Instance;
    [SerializeField] private IntVariable _mineDamage;
    [SerializeField] public FloatVariable _mineCd;
    [SerializeField] private IntVariable _shockwaveDamage;
    [SerializeField] private FloatVariable _shockwaveCd;
    [SerializeField] private FloatVariable _shockwaveRadius;
    [SerializeField] private FloatVariable _shockwaveHitboxRadius;
    [SerializeField] private FloatVariable _bulletCd;
    [SerializeField] private IntVariable _bulletDamage;
    [SerializeField] private IntVariable _missileDamage;
    [SerializeField] private FloatVariable _missileCd;
    [SerializeField] private IntVariable _rainDamage;
    [SerializeField] private FloatVariable _rainCd;

    private GameObject shockWaveGo;
    private bool initialized;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Awake()
    {
        Instance = this;
        initialized = false;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && EnemySpawner.Instance.currentRound == 1 &&
            TankstormGameManager.Instance.state.Value == TankstormGameManager.State.GamePlaying)
        {
            InitializeSkillsData();
        }
    }

    public void InitializeSkillsData()
    {
        _mineDamage.value = 9;
        _mineCd.value = 3;
        _shockwaveDamage.value = 4;
        _shockwaveCd.value = 3;
        _shockwaveRadius.value = 5;
        _shockwaveHitboxRadius.value = 5;
        if (Player.Instance != null && IsServer)
        {
            Player.Instance._moveSpeed.Value = 6;
            initialized = true;
        }
        _bulletCd.value = 1;
        _bulletDamage.value = 1;
        _missileDamage.value = 4;
        _missileCd.value = 3;
        _rainDamage.value = 1;
        _rainCd.value = 3;
    }

    private void Update()
    {
        if (shockWaveGo == null)
            shockWaveGo = GameObject.Find("Shockwave");

        if (!initialized)
            InitializeSkillsData();
    }

    #region Updates SO

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeRainCdServerRpc(float _rainCdUpgrade)
    {
        float initialValue = _rainCd.value;
        float newValue = initialValue * _rainCdUpgrade;
        UpgradeRainCdClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeRainCdClientRpc(float _rainCdUpgrade)
    {
        _rainCd.value = _rainCdUpgrade;
        Debug.Log("rain cd  is now " + _rainCd.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeRainDamageServerRpc()
    {
        int initialValue = _rainDamage.value;
        int newValue = initialValue + 1;
        UpgradeRainDamageClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeRainDamageClientRpc(int _rainDamageUpgrade)
    {
        _rainDamage.value = _rainDamageUpgrade;
        Debug.Log("rain damage is now " + _rainDamage.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeMissileCdServerRpc(float _missileCdUpgrade)
    {
        float initialValue = _missileCd.value;
        float newValue = initialValue * _missileCdUpgrade;
        UpgradeMissileCdClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeMissileCdClientRpc(float _missileCdUpgrade)
    {
        _missileCd.value = _missileCdUpgrade;
        Debug.Log("missile cd  is now " + _missileCd.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeMissileDamageServerRpc(float _missileDamageUpgrade)
    {
        int initialValue = _missileDamage.value;
        int newValue = (int)(initialValue * _missileDamageUpgrade);
        UpgradeMissileDamageClientRpc(newValue);
    }
    [ClientRpc]
    private void UpgradeMissileDamageClientRpc(int _missileDamageUpgrade)
    {
        _missileDamage.value = _missileDamageUpgrade;
        Debug.Log("Missile damage is now " + _missileDamage.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeBulletDamageServerRpc()
    {
        int initialValue = _bulletDamage.value;
        int newValue = initialValue + 1;
        UpgradeBulletDamageClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeBulletDamageClientRpc(int _bulletDamageUpgrade)
    {
        _bulletDamage.value = _bulletDamageUpgrade;
        Debug.Log("bullet damage is now " + _bulletDamage.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeBulletCdServerRpc(float _bulletCdUpgrade)
    {
        float initialValue = _bulletCd.value;
        float newValue = initialValue * _bulletCdUpgrade;
        UpgradeBulletCdClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeBulletCdClientRpc(float _bulletCdUpgrade)
    {
        _bulletCd.value = _bulletCdUpgrade;
        Debug.Log("movementspeed is now " + _bulletCd.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeMovementSpeedServerRpc(float _movementSpeedUpgrade)
    {
        //float initialValue = _movementSpeed.value;
        float initialValue = Player.Instance._moveSpeed.Value;
        float newValue = initialValue * _movementSpeedUpgrade;
        UpgradeMovementSpeedClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeMovementSpeedClientRpc(float _movementSpeedUpgrade)
    {
        Player.Instance._moveSpeed.Value = _movementSpeedUpgrade;
        Debug.Log("movementspeed is now " + Player.Instance._moveSpeed.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeShockwaveRadiusServerRpc(float _shockwaveRadiusUpgrade)
    {
        float initialValueVisual = _shockwaveRadius.value;
        float newValueVisual = initialValueVisual * _shockwaveRadiusUpgrade;
        float initialValueHitbox = _shockwaveHitboxRadius.value;
        float newValueHitbox = initialValueHitbox * _shockwaveRadiusUpgrade;
        UpgradeshockwaveRadiusClientRpc(newValueVisual, newValueHitbox);
    }

    [ClientRpc]
    private void UpgradeshockwaveRadiusClientRpc(float _shockwaveRadiusVisualUpgrade, float _shockwaveRadiusHitboxUpgrade)
    {
        _shockwaveRadius.value = _shockwaveRadiusVisualUpgrade;
        _shockwaveHitboxRadius.value = _shockwaveRadiusHitboxUpgrade;
        shockWaveGo.GetComponent<Shockwave>()._sphereCollider.radius = _shockwaveHitboxRadius.value;
        Debug.Log("shockwave radius is now " + _shockwaveRadius.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeShockwaveCdServerRpc(float _shockwaveCdUpgrade)
    {
        float initialValue = _shockwaveCd.value;
        float newValue = initialValue * _shockwaveCdUpgrade;
        UpgradeshockwaveCdClientRpc(newValue);
    }

    [ClientRpc]
    private void UpgradeshockwaveCdClientRpc(float _shockwaveCdUpgrade)
    {
        _shockwaveCd.value = _shockwaveCdUpgrade;
        Debug.Log("shockwave CD is now " + _shockwaveCd.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeShockwaveDamageServerRpc(float _shockwaveDamageUpgrade)
    {
        int initialValue = _shockwaveDamage.value;
        int newValue = (int)(initialValue * _shockwaveDamageUpgrade);
        UpgradeShockWaveDamageClientRpc(newValue);
    }
    [ClientRpc]
    private void UpgradeShockWaveDamageClientRpc(int _shockwaveDamageUpgrade)
    {
        _shockwaveDamage.value = _shockwaveDamageUpgrade;
        Debug.Log("Shockwave damage is now " + _shockwaveDamage.value);
    }

    [ServerRpc(RequireOwnership =false)]
    public void UpgradeMineCdServerRpc(float _mineCdUpgrade)
    {
        float initialValue = _mineCd.value;
        float newValue = initialValue * _mineCdUpgrade;
        UpgradeMineCdClientRpc(newValue);
    }
    
    [ClientRpc]
    private void UpgradeMineCdClientRpc(float _mineCdUpgrade)
    {
        _mineCd.value = _mineCdUpgrade;
        Debug.Log("Mine CD is now " + _mineCd.value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeMineDamageServerRpc(float _multiplierUpgrade)
    {
        int initialValue = _mineDamage.value;
        int newValue = (int)(initialValue * _multiplierUpgrade);
        UpdateUpgradeMineDamageClientRpc(newValue);
    }

    [ClientRpc]
    private void UpdateUpgradeMineDamageClientRpc(int newValue)
    {
        _mineDamage.value = newValue;
        Debug.Log("Mine damage is now " + _mineDamage.value);
    }
    #endregion
}
