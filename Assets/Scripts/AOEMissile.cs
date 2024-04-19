using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AOEMissile : NetworkBehaviour
{
    [SerializeField] private IntVariable _missileDamageToEnemies;
    private SphereCollider _sphereCollider;

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
            Debug.Log("touched an enemy with aoeCollider");
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                //enemy.SetCurrentHealthLossClientRpc(_missileDamageToEnemies);
                enemy.SetCurrentHealthLossServerRpc(_missileDamageToEnemies.value);
                enemy.GotHitServerRpc();
            }
        }
    }

    public void ActiveAOE()
    {
        _sphereCollider.enabled = true;
    }

}
