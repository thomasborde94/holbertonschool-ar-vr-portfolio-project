using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSingle : MonoBehaviour
{
    #region Show in Inspector
    [SerializeField] private float _multiplierMineBigUpgrade;
    [SerializeField] private float _multiplierMineSmallUpgrade;
    [SerializeField] private float _mineCdUpgrade;
    [SerializeField] private float _multiplierShockwaveBigUpgrade;
    [SerializeField] private float _multiplierShockwaveSmallUpgrade;
    [SerializeField] private float _shockwaveCdUpgrade;
    [SerializeField] private float _shockwaveRadiusBigUpgrade;
    [SerializeField] private float _shockwaveRadiusSmallUpgrade;
    [SerializeField] private float _movementSpeedBigUpgrade;
    [SerializeField] private float _movementSpeedSmallUpgrade;
    [SerializeField] private float _bulletCdUpgradeBig;
    [SerializeField] private float _bulletCdUpgradeSmall;
    [SerializeField] private float _multiplierMissileBigUpgrade;
    [SerializeField] private float _multiplierMissileSmallUpgrade;
    [SerializeField] private float _multiplierMissileCdUpgrade;
    [SerializeField] private float _multiplierRainCdUpgrade;

    public int _upgradeCost;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] public TextMeshProUGUI _descriptionText;
    [HideInInspector] public Image _image;
    public bool isAvailable;

    #endregion
    private bool spentCoins;

    private void Awake()
    {
        _image = GetComponent<Image>();
        isAvailable = true;
        _costText.text = _upgradeCost.ToString();
        spentCoins = false;
    }
    #region Upgrade buttons
    public void RainCd()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeRainCdServerRpc(_multiplierRainCdUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void RainDamage()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeRainDamageServerRpc();
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void MissileCd()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMissileCdServerRpc(_multiplierMissileCdUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    public void MissileDamageBig()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMissileDamageServerRpc(_multiplierMissileBigUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    public void MissileDamageSmall()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMissileDamageServerRpc(_multiplierMissileSmallUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void BulletDamage()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeBulletDamageServerRpc();
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void BulletCdBig()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeBulletCdServerRpc(_bulletCdUpgradeBig);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    public void BulletCdSmall()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeBulletCdServerRpc(_bulletCdUpgradeSmall);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void MovementSpeedBig()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMovementSpeedServerRpc(_movementSpeedBigUpgrade);
                DisableUpgrade();
            }
            spentCoins= false;
        }
    }

    public void MovementSpeedSmall()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMovementSpeedServerRpc(_movementSpeedSmallUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void ShockwaveRadiusBig()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeShockwaveRadiusServerRpc(_shockwaveRadiusBigUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    public void ShockwaveRadiusSmall()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeShockwaveRadiusServerRpc(_shockwaveRadiusSmallUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void ShockwaveCd()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeShockwaveCdServerRpc(_shockwaveCdUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void ShockwaveDamageBig()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeShockwaveDamageServerRpc(_multiplierShockwaveBigUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    public void ShockwaveDamageSmall()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeShockwaveDamageServerRpc(_multiplierShockwaveSmallUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    public void MineDamageBig()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMineDamageServerRpc(_multiplierMineBigUpgrade);
                DisableUpgrade();
            }
            spentCoins=false;
        }
    }

    public void MineDamageSmall()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMineDamageServerRpc(_multiplierMineSmallUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }

    public void MineCd()
    {
        if (isAvailable)
        {
            SpendCoins();
            if (spentCoins)
            {
                SFXManager.Instance.PlaySFX(8);
                UpdateCost();
                DataManager.Instance.UpgradeMineCdServerRpc(_mineCdUpgrade);
                DisableUpgrade();
            }
            spentCoins = false;
        }
    }
    #endregion


    private void DisableUpgrade()
    {
        Color currentColor = _image.color;
        currentColor.a = 0.5f;
        _image.color = currentColor;

        Color currentTextColor = _descriptionText.color;
        currentTextColor.a = 0.5f;
        _descriptionText.color = currentTextColor;

        isAvailable = false;
    }

    private void SpendCoins()
    {
        if (Player.Instance.coinAmount >= _upgradeCost)
        {
            spentCoins = true;
            Player.Instance.coinAmount -= _upgradeCost;
            CoinsUI.Instance.UpdateCoinAmout();
        }
        else
            Debug.Log("not enough money");
    }

    private void UpdateCost()
    {
        _upgradeCost += (int)(_upgradeCost * (50.0f / 100.0f));
        _costText.text = _upgradeCost.ToString();
    }
}
