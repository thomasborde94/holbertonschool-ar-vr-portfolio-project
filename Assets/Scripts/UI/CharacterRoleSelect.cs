using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRoleSelect : MonoBehaviour
{
    [SerializeField] private int roleId;
    [SerializeField] private GameObject selectedGameobject;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SFXManager.Instance.PlaySFX(0);
            TankstormGameMultiplayer.Instance.ChangePlayerRole(roleId);
        });
    }

    private void Start()
    {
        TankstormGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += TankstormGameMultiplayer_OnPlayerDataNetworkListChanged;

        UpdateIsSelected();
    }

    private void TankstormGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    // Display the "isSelected" icon on UI
    private void UpdateIsSelected()
    {
        if (TankstormGameMultiplayer.Instance.GetPlayerData().roleId == roleId)
        {
            selectedGameobject.SetActive(true);
        }
        else
        {
            selectedGameobject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        TankstormGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= TankstormGameMultiplayer_OnPlayerDataNetworkListChanged;
    }

}
