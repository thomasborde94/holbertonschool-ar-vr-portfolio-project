using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject spawnParticlesPrefab;
    [SerializeField] private float _timeBetweenSpawnRank = 4f;
    [SerializeField] private float _spawnAreaWidth = 8f;
    [SerializeField] private float _spawnAreaLength = 8f;

    [SerializeField] public EnemyListSO _enemyListSO;

    public int currentRound = 1;
    private float particlesTimer = 0f;
    private float enemyTimer = 0f;
    private float _timeBetweenSpawn = 4f;
    private float _despawnTimer = 0f;

    private Vector3 spawnPosition;
    private NetworkObject particlesNO;

    private void Start()
    {
        _timeBetweenSpawn = _timeBetweenSpawnRank * (1.5f / currentRound);
    }

    private void Update()
    {
        particlesTimer += Time.deltaTime;
        enemyTimer += Time.deltaTime;
        _despawnTimer += Time.deltaTime;

        if (particlesTimer >= _timeBetweenSpawn)
        {
            spawnPosition = GetRandomSpawnPosition();
            SpawnParticlesServerRpc();
            particlesTimer = 0f;
            _despawnTimer = 0f;

        }
        if (enemyTimer >= _timeBetweenSpawn + 0.2f)
        {
            SpawnEnemyDelayServerRpc();
            particlesTimer = 0f;
            enemyTimer = 0f;
            _despawnTimer = 0f;
        }
        if (particlesNO != null && _despawnTimer >= 1f)
            particlesNO.Despawn(true);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-_spawnAreaWidth / 2f, _spawnAreaWidth / 2f);
        float randomZ = Random.Range(-_spawnAreaLength / 2f, _spawnAreaLength / 2f);

        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, randomZ);

        return spawnPosition;
    }

    [ServerRpc(RequireOwnership =true)]
    private void SpawnParticlesServerRpc()
    {
        // spawns particles of the enemy
        GameObject particles = Instantiate(spawnParticlesPrefab, spawnPosition, Quaternion.identity);
        particlesNO = particles.GetComponent<NetworkObject>();
        particlesNO.Spawn(true);
    }

    [ServerRpc(RequireOwnership = true)]
    private void SpawnEnemyDelayServerRpc()
    {
        // Spawns the enemy
        int enemyIndex = Random.Range(0, Mathf.Min(enemyPrefabs.Count, currentRound));
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);
        NetworkObject enemyNO = enemy.GetComponent<NetworkObject>();
        enemyNO.Spawn(true);
        _enemyListSO.AddEnemy(enemy);
    }
}
