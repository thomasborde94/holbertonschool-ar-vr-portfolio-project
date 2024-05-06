using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AOEMissile : NetworkBehaviour
{
    [SerializeField] private IntVariable _missileDamageToEnemies;
    private SphereCollider _sphereCollider;

    #region Unity Lifecycle

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        _sphereCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetCurrentHealthLossServerRpc(_missileDamageToEnemies.value);
                enemy.GotHitServerRpc();
            }
        }
    }

    #endregion

    // Enables AOE damage of the missile
    public void ActiveAOE()
    {
        _sphereCollider.enabled = true;
    }
}
