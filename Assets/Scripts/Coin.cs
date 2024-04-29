using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    [SerializeField] private GameObject _coinGameObject;

    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = _coinGameObject.GetComponent<NetworkObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.coinAmount++;
            CoinsUI.Instance.UpdateCoinAmout();

            DespawnWithDelay(2f);
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            if (TankstormGameManager.Instance.state.Value == TankstormGameManager.State.ChoosingSkills)
                DespawnWithDelay(2f);
        }
        
    }

    public void DespawnWithDelay(float delay)
    {
        StartCoroutine(DespawnCoroutine(delay));
    }
    private IEnumerator DespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
    }
}
