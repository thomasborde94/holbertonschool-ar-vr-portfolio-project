using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private int _bulletDamageToEnemies = 4;
    [SerializeField] private int _bulletDamageToPlayer = 1;


    [SerializeField] private bool isMissile;
    [SerializeField] private AOEMissile _aoeMissile;
    [SerializeField] private SphereCollider _aoeCollider;
    [SerializeField] private GameObject _missileExplosionParticles;

    [SerializeField] private EnemyListSO _enemyListSO;

    #region Private

    private Rigidbody _rigidbody;
    private Transform _transform;
    private float _bulletSpeed;
    private NetworkObject networkObject;

    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        networkObject = GetComponent<NetworkObject>();
    }

    private void FixedUpdate()
    {
        // Moves the bullet when shot
        Vector3 bulletVelocity = transform.forward * _bulletSpeed;
        Vector3 currentBulletPosition = _transform.position;
        Vector3 bulletPosition = currentBulletPosition + bulletVelocity * Time.fixedDeltaTime;
        _rigidbody.MovePosition(bulletPosition);
    }

    public void Shoot(float Speed)
    {
        _bulletSpeed = Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("hit player");
            Player.Instance.SetCurrentHealthLoss(_bulletDamageToPlayer);
        }

        if (other.CompareTag("Enemy"))
        {
            if (!isMissile)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                    EnemyHitWithBulletServerRpc(enemy.index);

            }
            else
            {
                _aoeMissile.ActiveAOE();
                GameObject explosion = Instantiate(_missileExplosionParticles, transform.position, Quaternion.identity);
                Destroy(explosion.gameObject, 1f);
                Destroy(gameObject, 0.1f);
            }
        }
    }
    
    [ServerRpc(RequireOwnership =false)]
    private void EnemyHitWithBulletServerRpc(int enemyListIndex)
    {
        GameObject enemy = _enemyListSO.GetEnemy(enemyListIndex);
        if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.SetCurrentHealthLoss(_bulletDamageToEnemies);
            enemyScript.GotHit();
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
