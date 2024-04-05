using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Mine : NetworkBehaviour
{
    [SerializeField] private int _mineDamage = 7;
    [SerializeField] private GameObject _explosionParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.SetCurrentHealthLoss(_mineDamage);
            enemy.GotHit();

            Instantiate(_explosionParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
