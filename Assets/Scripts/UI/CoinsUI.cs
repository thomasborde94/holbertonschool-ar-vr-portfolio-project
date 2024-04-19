using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    public static CoinsUI Instance;

    [SerializeField] private TextMeshProUGUI _coinText;

    private bool updatedCoins;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        updatedCoins = false;
    }

    private void Update()
    {
        if (Player.Instance != null && !updatedCoins)
            _coinText.text = Player.Instance.coinAmount.ToString();
    }

    public void UpdateCoinAmout()
    {
        _coinText.text = Player.Instance.coinAmount.ToString();
    }
}
