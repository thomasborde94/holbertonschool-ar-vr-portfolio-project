using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Users;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    public static Player Instance {  get; private set; }


    [Header("Player")]

    public NetworkVariable<float> _moveSpeed = new NetworkVariable<float>(10f);
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _towerRotationSpeed = 10f;
    public float _maxHealth = 10;
    public NetworkVariable<float> _currentHealth = new NetworkVariable<float>(10f);

    [Header("Bullet")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private FloatVariable _bulletCd;
    [SerializeField] private Transform _cannon;
    [Header("Mine")]
    [SerializeField] private GameObject _minePrefab;
    [SerializeField] private FloatVariable _mineCooldown;
    [Header("AOEMissile")]
    [SerializeField] private Bullet _aoeMissilePrefab;
    [SerializeField] private float _aoeMissileSpeed = 5f;
    [SerializeField] private FloatVariable _aoeMissileCooldown;
    [Header("Rain")]
    [SerializeField] private FloatVariable _rainCooldown;
    [SerializeField] private float _rainDuration = 3f;
    [SerializeField] private GameObject _rainPrefab;
    [SerializeField] private LayerMask targetLayer;
    [Header("Shockwave")]
    [SerializeField] private Shockwave _shockwave;
    [SerializeField] private FloatVariable _shockwaveCooldown;
    [Header("Others")]
    [SerializeField] private Transform _tower;
    [Tooltip("Player will collide with this layer")][SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCamera;

    public int coinAmount = 0;



    private float _nextShotTime = 0f;
    [HideInInspector] public float _nextAOEMissileTime = 2f;
    [HideInInspector] public float _nextRainTime = 2f;
    [HideInInspector] public float _nextShockwaveTime = 2f;
    [HideInInspector] public float _nextMineTime = 2f;
    private float _delayBeforeBulletDespawn = 2.5f;
    private bool startedPlayingTankSound = false;


    private void Awake()
    {
        Instance = this;
        coinAmount = 0;
    }

    void Start()
    {
        _currentHealth.Value = _maxHealth;
        inputHandler = PlayerInputHandler.Instance;

        _cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (_cinemachineCamera == null)
            Debug.Log("could not find cinemachine camera");
        else
        {
            _cinemachineCamera.m_LookAt = transform;
            _cinemachineCamera.m_Follow = transform;
        }
    }

    void Update()
    {
        if (PlayerRoleString() == "Driver" && TankstormGameManager.Instance.state.Value == TankstormGameManager.State.GamePlaying)
        {
            HandleMovementServerAuth();
            HandleMineServerAuth(inputHandler.Skill1Input);
            HandleShockwaveServerAuth(inputHandler.Skill2Input);
        }

        else if (PlayerRoleString() == "Shooter" && TankstormGameManager.Instance.state.Value == TankstormGameManager.State.GamePlaying)
        {
            HandleTowerRotationServerAuth();
            HandleShootingServerAuth(inputHandler.ShootInput);
            HandleAOEMissileServerAuth(inputHandler.Skill1Input);
            HandleRainServerAuth(inputHandler.Skill2Input);
        }
    }



    private void HandleMovementServerAuth()
    {
        Vector2 inputVector = inputHandler.MoveInput;
        HandleMovementServerRpc(inputVector);
    }

    [ServerRpc(RequireOwnership =false)]
    private void HandleMovementServerRpc(Vector2 inputVector)
    {
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        if (inputVector != Vector2.zero && !startedPlayingTankSound)
        {
            SFXManager.Instance.PlaySFX(10);
            startedPlayingTankSound = true;
        }
        else if (inputVector == Vector2.zero)
        {
            SFXManager.Instance.StopSFX(10);
            startedPlayingTankSound = false;
        }
        float moveDistance = _moveSpeed.Value * Time.deltaTime;
        float playerRadius = 0.7f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection,
            Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = (moveDirection.x < -.5f || moveDirection.x > +.5f) && !Physics.BoxCast(transform.position,
                Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                // Can move only on the X
                moveDirection = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = (moveDirection.z < -.5f || moveDirection.z > +.5f) && !Physics.BoxCast(transform.position,
                    Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDirection = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }
        if (moveDirection != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * _rotateSpeed);
    }


    private void HandleTowerRotationServerAuth()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
        HandleTowerRotationServerRpc(mousePosition);
    }


    [ServerRpc(RequireOwnership = false)]
    private void HandleTowerRotationServerRpc(Vector3 mousePosition)
    {
        // Get mousePosition on X-Z plane
        Vector3 mousePositionHorizontal = new Vector3(mousePosition.x, _tower.position.y, mousePosition.z);

        // Get direction between tower position and mouse position
        Vector3 directionToMouse = mousePositionHorizontal - _tower.position;

        // Apply rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse, Vector3.up);
        _tower.rotation = Quaternion.Slerp(_tower.rotation, targetRotation, _towerRotationSpeed * Time.deltaTime);
    }

    private void HandleShootingServerAuth(bool shootInput)
    {
        if (shootInput)
        {
            HandleShootingServerRpc();
        }
        if (_nextShotTime < 5f)
        {
            IncreaseNextShotTimeServerRpc();
        }
        
    }
    [ServerRpc(RequireOwnership =false)]
    private void IncreaseNextShotTimeServerRpc()
    {
        _nextShotTime += Time.deltaTime;
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncreaseNextAOEMissileTimeServerRpc()
    {
        IncreaseNextAOEMissileTimeClientRpc();
        
    }

    [ClientRpc]
    private void IncreaseNextAOEMissileTimeClientRpc()
    {
        _nextAOEMissileTime += Time.deltaTime;
    }
    [ServerRpc(RequireOwnership = false)]
    private void HandleShootingServerRpc()
    {
        if (_nextShotTime >= _bulletCd.value)
        {
            SFXManager.Instance.PlaySFX(7);
            Firebullet();
            _nextShotTime = 0f;
        }
    }

    private void Firebullet()
    {
        Bullet newBullet = Instantiate(_bulletPrefab, _cannon.position, _cannon.rotation);
        NetworkObject newbulletNO = newBullet.GetComponent<NetworkObject>();
        newbulletNO.Spawn(true);
        newBullet.Shoot(_bulletSpeed);
        newBullet.DespawnWithDelay(_delayBeforeBulletDespawn);
    }

    private void HandleAOEMissileServerAuth(bool skill1Input)
    {
        if (skill1Input)
        {
            HandleFireAOEMissileServerRpc();
        }
        if (_nextAOEMissileTime < 5f)
        {
            IncreaseNextAOEMissileTimeServerRpc();
        }  
    }

    private void HandleShockwaveServerAuth(bool skill2Input)
    {
        if (skill2Input)
        {
            HandleShockwaveServerRpc();
        }
        if (_nextShockwaveTime < 5f)
            IncreaseNextShockwaveTimeServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void HandleShockwaveServerRpc()
    {
        if (_nextShockwaveTime >= _shockwaveCooldown.value)
        {
            SFXManager.Instance.PlaySFX(2);
            CallBlastClientRpc();
            ResetNextShockwaveTimeClientRpc();
        }
    }
    [ClientRpc]
    private void ResetNextShockwaveTimeClientRpc()
    {
        _nextShockwaveTime = 0f;
    }

    [ClientRpc]
    private void CallBlastClientRpc()
    {
        _shockwave._lineRenderer.enabled = true;
        StartCoroutine(_shockwave.Blast());
    }
    [ServerRpc(RequireOwnership =false)]
    private void IncreaseNextShockwaveTimeServerRpc()
    {
        IncreaseNextShockwaveTimeClientRpc();
    }
    [ClientRpc]
    private void IncreaseNextShockwaveTimeClientRpc()
    {
        _nextShockwaveTime += Time.deltaTime;
    }
    private void HandleMineServerAuth(bool skill1Input)
    {
        if (skill1Input)
        {
            HandleMineServerRpc();
        }
        if (_nextMineTime < 5f)
            IncreaseNextMineTimeServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void HandleMineServerRpc()
    {
        if (_nextMineTime >= _mineCooldown.value)
        {
            SFXManager.Instance.PlaySFX(4);
            GameObject mine = Instantiate(_minePrefab, transform.position, Quaternion.identity);
            NetworkObject mineNo = mine.GetComponent<NetworkObject>();
            mineNo.Spawn(true);
            ResetNextMineTimeClientRpc();
            
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void IncreaseNextMineTimeServerRpc()
    {
        IncreaseNextMineTimeClientRpc();
        
    }
    [ClientRpc]
    private void ResetNextMineTimeClientRpc()
    {
        _nextMineTime = 0f;
    }
    [ClientRpc]
    private void IncreaseNextMineTimeClientRpc()
    {
        if (_nextMineTime >= 10f)
            _nextMineTime = 10f;
        else
            _nextMineTime += Time.deltaTime;
    }

    private void HandleRainServerAuth(bool skill2Input)
    {
        if (skill2Input)
        {
            Vector3 mousePosition = inputHandler.GetLocalMousePosition();
            HandleRainServerRpc(mousePosition);
        }
        if (_nextRainTime < 5f)
        {
            IncreaseNextRainTimeServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void HandleRainServerRpc(Vector3 mousePosition, ServerRpcParams serverRpcParams = default)
    {
        if (_nextRainTime >= _rainCooldown.value)
        {
            StartCoroutine(PlayRainSound());
            
            FireRain(mousePosition);
            ResetNextRainTimeClientRpc();
            
        }
    }

    private IEnumerator PlayRainSound()
    {
        SFXManager.Instance.PlaySFX(5);
        yield return new WaitForSeconds(3f);
        SFXManager.Instance.StopSFX(5);
    }
    [ClientRpc]
    private void ResetNextRainTimeClientRpc()
    {
        _nextRainTime = 0;
    }


    private void FireRain(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            if (hit.collider != null)
            {
                GameObject rain = Instantiate(_rainPrefab, hit.point, Quaternion.identity);
                NetworkObject rainNO = rain.GetComponent<NetworkObject>();
                rainNO.Spawn(true);
                //Destroy(rain, _rainDuration);
                StartCoroutine(DespawnCoroutine(rainNO, _rainDuration));
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncreaseNextRainTimeServerRpc()
    {
        IncreaseNextRainTimeClientRpc();
        
    }

    [ClientRpc]
    private void IncreaseNextRainTimeClientRpc()
    {
        _nextRainTime += Time.deltaTime;
    }

    [ServerRpc(RequireOwnership =false)]
    private void HandleFireAOEMissileServerRpc()
    {
        if (_nextAOEMissileTime >= _aoeMissileCooldown.value)
        {
            SFXManager.Instance.PlaySFX(1);
            FireAOEMissile();
            ResetNextAOEMissileTimeClientRpc();
            
        }
    }

    [ClientRpc]
    private void ResetNextAOEMissileTimeClientRpc()
    {
        _nextAOEMissileTime = 0f;
    }

    private void FireAOEMissile()
    {
        Bullet newMissile = Instantiate(_aoeMissilePrefab, _cannon.position, _cannon.rotation);
        NetworkObject newMissileNO = newMissile.GetComponent<NetworkObject>();
        newMissileNO.Spawn(true);
        newMissile.Shoot(_aoeMissileSpeed);
        newMissile.DespawnWithDelay(_delayBeforeBulletDespawn);
    }

    public float GetCurrentHealthPart()
    {
        return (_currentHealth.Value / _maxHealth);
    }

    public void SetCurrentHealthLoss(float healthAmountLost)
    {
        if (IsServer)
        {
            SFXManager.Instance.PlaySFX(6);
            _currentHealth.Value -= healthAmountLost;
        }
    }

    // Gets the role of the player
    public string PlayerRoleString()
    {
        int roleId = TankstormGameMultiplayer.Instance.GetPlayerData().roleId;
        if (roleId == 0)
        {
            return "Driver";
        }
        if (roleId == 1)
            return "Shooter";
        else
            return null;
    }

    public void DespawnWithDelay(NetworkObject networkObject, float delay)
    {
        StartCoroutine(DespawnCoroutine(networkObject, delay));
    }
    private IEnumerator DespawnCoroutine(NetworkObject networkObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("an enemy hit the player");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("an enemy hit the player");
        }
    }
    */

}
