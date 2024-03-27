using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DebugScript : NetworkBehaviour
{
    private void Update()
    {
        Debug.Log(PlayerRoleString());
    }
    public string PlayerRoleString()
    {
        int roleId = TankstormGameMultiplayer.Instance.GetPlayerData().roleId;
        if (roleId == 0)
        {
            return "Driver";
        }
        if (roleId == 1)
            return "Shooter";
        else
            return "ERROR";
    }
}
