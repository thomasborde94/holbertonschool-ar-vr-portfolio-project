using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Rain : NetworkBehaviour
{
    [SerializeField] private float _damageTickCooldown = 1f;
    [SerializeField] private int _rainDamage = 1;

    private float _nextTickTime = 0f;

    private void Update()
    {
        _nextTickTime += Time.deltaTime;

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                DamageEnemy(enemy);
            }
        }
    }

    private void DamageEnemy(Enemy enemy)
    {
        if (_nextTickTime >= _damageTickCooldown)
        {
            enemy.GotHit();
            enemy.SetCurrentHealthLoss(_rainDamage);
            _nextTickTime = 0f;
        }
    }
}
