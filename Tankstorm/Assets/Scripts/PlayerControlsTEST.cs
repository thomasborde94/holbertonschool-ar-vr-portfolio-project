using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlsTEST : MonoBehaviour
{
    public static PlayerControlsTEST Instance;
    public float speed = 10f;

    public float _maxHealth = 10;
    public float _currentHealth = 10;

    [Header("Weapons")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _delayBetweenBulletShots = 1f;
    [SerializeField] private Transform _cannon;
    [SerializeField] private float _destroyTime = 2f;

    [Header("Mine")]
    [SerializeField] private GameObject _minePrefab;
    [SerializeField] private float _mineCooldown = 2f;
    [Header("Shockwave")]
    [SerializeField] private Shockwave _shockwave;
    [SerializeField] private float _shockwaveCooldown = 3f;
    [Header("AOEMissile")]
    [SerializeField] private Bullet _aoeMissilePrefab;
    [SerializeField] private float _aoeMissileSpeed = 5f;
    [SerializeField] private float _aoeMissileCooldown = 2f;
    [Header("Rain")]
    [SerializeField] private float _rainCooldown = 3f;
    [SerializeField] private float _rainDuration = 3f;
    [SerializeField] private GameObject _rainPrefab;
    [SerializeField] private LayerMask targetLayer;
    [Header("Others")]
    [Tooltip("Player will collide with this layer")][SerializeField] private LayerMask collisionsLayerMask;


    public bool isDriver = false;
    public bool isShooter = false;

    public event EventHandler OnShootPressed;

    private float _nextShotTime = 0f;
    private float _nextMineTime = 2f;
    private float _nextShockwaveTime = 2f;
    private float _nextAOEMissileTime = 2f;
    private float _nextRainTime = 2f;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
        OnShootPressed += Player_OnShootPressed;
    }
    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position = transform.position + new Vector3(x, 0, z) * speed * Time.deltaTime;
        HandleRotation();
        HandleShooting();

        if (isDriver)
        {
            HandleMine();
            HandleShockWave();
        }

        if (isShooter)
        {
            HandleAOEMissile();
            HandleRain();
        }
    }

    private void HandleRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, Camera.main.transform.position.y));
        Vector3 mousePositionHorizontal = new Vector3(mousePosition.x, transform.position.y, mousePosition.z);

        // Get direction between tower position and mouse position
        Vector3 directionToMouse = mousePositionHorizontal - transform.position;

        // Apply rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (inputHandler.ShootInput)
        {
            OnShootPressed?.Invoke(this, EventArgs.Empty);
        }
        if (_nextShotTime < 5f)
            _nextShotTime += Time.deltaTime;
    }

    private void Player_OnShootPressed(object sender, EventArgs e)
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

    private void FireAOEMissile()
    {
        Bullet newMissile = Instantiate(_aoeMissilePrefab, _cannon.position, _cannon.rotation);
        newMissile.Shoot(_aoeMissileSpeed);

        Destroy(newMissile.gameObject, _destroyTime);
    }

    private void HandleMine()
    {
        _nextMineTime += Time.deltaTime;
        if (_nextMineTime >= 10f)
            _nextMineTime = 10f;
        if (inputHandler.Skill1Input)
        {
            if (_nextMineTime >= _mineCooldown)
            {
                Instantiate(_minePrefab, transform.position, Quaternion.identity);
                _nextMineTime = 0f;
            }
        }
    }

    private void HandleShockWave()
    {
        _nextShockwaveTime += Time.deltaTime;
        if (_nextShockwaveTime >= 8f)
            _nextShockwaveTime = 8f;
        if (inputHandler.Skill2Input)
        {
            if (_nextShockwaveTime >= _shockwaveCooldown)
            {
                _shockwave._lineRenderer.enabled = true;
                StartCoroutine(_shockwave.Blast());
                _nextShockwaveTime = 0f;
            }
        }
    }

    private void HandleAOEMissile()
    {
        _nextAOEMissileTime += Time.deltaTime;
        if (_nextAOEMissileTime >= 6f)
            _nextAOEMissileTime = 6f;
        if (inputHandler.Skill1Input)
        {
            if (_nextAOEMissileTime >= _aoeMissileCooldown)
            {
                FireAOEMissile();
                _nextAOEMissileTime = 0f;
            }
        }

    }

    private void HandleRain()
    {
        _nextRainTime += Time.deltaTime;
        if (_nextRainTime >= 8f)
            _nextRainTime = 8f;
        if (inputHandler.Skill2Input)
        {
            if (_nextRainTime >= _rainCooldown)
                FireRain();
            _nextRainTime = 0f;
        }
    }

    private void FireRain()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            if (hit.collider != null)
            {
                GameObject rain = Instantiate(_rainPrefab, hit.point, Quaternion.identity);
                Destroy(rain, _rainDuration);
            }
        }
    }
    public float GetCurrentHealthPart()
    {
        return (_currentHealth / _maxHealth);
    }

    public void SetCurrentHealthLoss(float healthAmountLost)
    {
        _currentHealth -= healthAmountLost;
    }
}
