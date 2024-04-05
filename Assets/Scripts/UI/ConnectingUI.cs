using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        TankstormGameMultiplayer.Instance.OnTryingToJoinGame += TankstormGameMultiplayer_OnTryingToJoinGame;
        TankstormGameMultiplayer.Instance.OnFailedToJoinGame += TankstormGameMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void TankstormGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void TankstormGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
        TankstormGameMultiplayer.Instance.OnTryingToJoinGame -= TankstormGameMultiplayer_OnTryingToJoinGame;
        TankstormGameMultiplayer.Instance.OnFailedToJoinGame -= TankstormGameMultiplayer_OnFailedToJoinGame;
    }
}
