using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Private

    private Rigidbody _rigidbody;
    private Transform _transform;
    private float _bulletSpeed;

    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
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
}
