using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockUI : MonoBehaviour
{
    public static GameClockUI Instance { get; private set; }

    [SerializeField] private Image timerImage;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        timerImage.fillAmount = TankstormGameManager.Instance.GetGamePlayingTimerNormalized();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
