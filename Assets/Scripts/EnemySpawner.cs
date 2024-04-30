using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public static EnemySpawner Instance;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject spawnParticlesPrefab;
    [SerializeField] public float _timeBetweenSpawnRank = 2f;
    [SerializeField] private float _spawnAreaWidth = 40f;
    [SerializeField] private float _spawnAreaLength = 40f;

    [SerializeField] public EnemyListSO _enemyListSO;
    public bool shouldSpawn = true;
    private int index = 0;

    public int currentRound = 1;
    private float particlesTimer = 0f;
    private float enemyTimer = 0f;
    [HideInInspector] public float _timeBetweenSpawn;
    private float _despawnTimer = 0f;

    private Vector3 spawnPosition;
    private NetworkObject particlesNO;
    private bool shouldSpawnBoss;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _timeBetweenSpawn = _timeBetweenSpawnRank * (1.5f / currentRound);
        _enemyListSO.Clear();
        shouldSpawnBoss = false;
        currentRound = 1;
    }

    private void Update()
    {
        if (!IsServer)
            return;


        if (shouldSpawn)
        {
            particlesTimer += Time.deltaTime;
            enemyTimer += Time.deltaTime;
            _despawnTimer += Time.deltaTime;

            if (particlesTimer >= _timeBetweenSpawn)
            {
                spawnPosition = GetRandomSpawnPosition();
                SpawnParticlesServerRpc(spawnPosition);
                particlesTimer = 0f;
                _despawnTimer = 0f;
            }

            if (enemyTimer >= _timeBetweenSpawn + 0.2f)
            {
                SpawnEnemyDelayServerRpc(spawnPosition);
                particlesTimer = 0f;
                enemyTimer = 0f;
                _despawnTimer = 0f;
            }
            if (particlesNO != null && _despawnTimer >= 1f)
                particlesNO.Despawn(true);
        }
        if (shouldSpawn && currentRound == 5 && !shouldSpawnBoss)
            SpawnBoss();
    }

    private void SpawnBoss()
    {
        SpawnParticlesServerRpc(spawnPosition);
        spawnPosition = GetRandomSpawnPosition();
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        NetworkObject bossNO = boss.GetComponent<NetworkObject>();
        bossNO.Spawn();
        shouldSpawnBoss = true;
    }


    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-_spawnAreaWidth / 2f, _spawnAreaWidth / 2f);
        float randomZ = Random.Range(-_spawnAreaLength / 2f, _spawnAreaLength / 2f);

        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, randomZ);

        return spawnPosition;
    }


    [ServerRpc(RequireOwnership = true)]
    private void SpawnParticlesServerRpc(Vector3 spawnPosition)
    {
        // spawns particles of the enemy
        GameObject particles = Instantiate(spawnParticlesPrefab, spawnPosition, Quaternion.identity);
        particlesNO = particles.GetComponent<NetworkObject>();
        particlesNO.Spawn(true);
    }


    [ServerRpc(RequireOwnership = true)]
    private void SpawnEnemyDelayServerRpc(Vector3 spawnPosition)
    {
        // Spawns the enemy

        int enemyIndex = Random.Range(0, Mathf.Min(enemyPrefabs.Count, currentRound));
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);
        NetworkObject enemyNO = enemy.GetComponent<NetworkObject>();
        enemyNO.Spawn(true);
        _enemyListSO.AddEnemy(enemy);
        enemy.GetComponent<Enemy>().index = index;
        index++;
    }

    [ServerRpc]
    public void KillAllEnemiesServerRpc()
    {
        for (int i = _enemyListSO.Count() - 1; i >= 0; i--)
        {
            var enemy = _enemyListSO.GetEnemy(i);
            if (enemy != null && enemy.gameObject != null && enemy.gameObject.activeSelf)
            {
                enemy.gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
        _enemyListSO.Clear();
    }
}
