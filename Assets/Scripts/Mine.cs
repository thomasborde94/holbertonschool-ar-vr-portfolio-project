using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Mine : NetworkBehaviour
{
    [SerializeField] private IntVariable _mineDamage;
    [SerializeField] private GameObject _explosionParticles;

    private NetworkObject networkObject;
    private float _destructionDelay = 1f;
    private bool taggedEnemy;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
    }

    private void Start()
    {
        taggedEnemy = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;
        if (other.CompareTag("Enemy") && !taggedEnemy)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                //enemy.SetCurrentHealthLossClientRpc(_mineDamage.value);
                enemy.SetCurrentHealthLossServerRpc(_mineDamage.value);
                enemy.GotHitServerRpc();
            }
            
            GameObject particles = Instantiate(_explosionParticles, transform.position, Quaternion.identity);
            NetworkObject particlesNO = particles.GetComponent<NetworkObject>();
            Debug.Log("should have instantiated mine particles");
            particlesNO.Spawn(true);
            DespawnWithDelay(_destructionDelay);
            DespawnParticlesWithDelay(particlesNO, _destructionDelay);
            taggedEnemy = true;
        }
    }

    private void DespawnParticlesWithDelay(NetworkObject particles, float delay)
    {
        StartCoroutine(DespawnParticlesCoroutine(particles, delay));
    }
    private IEnumerator DespawnParticlesCoroutine(NetworkObject particles, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (particles.IsSpawned)
        {
            particles.Despawn(true);
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
            networkObject.Despawn(true);
        }
    }

}
