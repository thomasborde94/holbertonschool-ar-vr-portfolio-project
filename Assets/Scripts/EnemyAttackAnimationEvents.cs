using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAttackAnimationEvents : MonoBehaviour
{
    [SerializeField] private Collider attackHitbox;

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _delayBetweenBulletShots = 1.4f;
    [SerializeField] private Transform _cannon;
    [SerializeField] private float _destroyTime = 2f;

    private float _nextShotTime = 1f;

    private void Update()
    {
        if (_nextShotTime < 5f)
            _nextShotTime += Time.deltaTime;
    }
    public void EnableAttackHitbox()
    {
        attackHitbox.enabled = true;
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.enabled = false;
    }

    public void EnemyShoot()
    {
        if (_nextShotTime >= _delayBetweenBulletShots)
        {
            Firebullet();
            _nextShotTime = 0f;
        }
    }

    private void Firebullet()
    {
        Bullet newBullet = Instantiate(_bulletPrefab, _cannon.position, _cannon.rotation);
        newBullet.Shoot(_bulletSpeed);

        Destroy(newBullet.gameObject, _destroyTime);
    }


}
