using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForOtherPlayersUI : MonoBehaviour
{
    private void Start()
    {
        TankstormGameManager.Instance.OnLocalPlayerReadyChanged += TankstormGameManager_OnLocalPlayerReadyChanged;
        Hide();
    }

    private void TankstormGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (TankstormGameManager.Instance.IsLocalPlayerReady())
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
