using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Users;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    public static Player Instance {  get; private set; }

    public float speed = 10f;

    [Header("Player")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _towerRotationSpeed = 10f;

    [Header("Weapons")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _delayBetweenBulletShots = 1f;
    [SerializeField] private Transform _cannon;
    [SerializeField] private float _destroyTime = 2f;
    [Header("Others")]
    [SerializeField] private Transform _tower;
    [Tooltip("Player will collide with this layer")][SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private PlayerInputHandler inputHandler;



    private float _nextShotTime = 0f;
    private PlayerData playerData;

    public event EventHandler OnShootPressed;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        /*
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        */
    }

    void Start()
    {
        inputHandler = PlayerInputHandler.Instance;

        OnShootPressed += Player_OnShootPressed;
    }

    void Update()
    {
        /*
        if (IsClient)
            Debug.Log("Créé côté client");
        if (IsServer)
            Debug.Log("Créé côté server");
        //if (PlayerRoleString() == "Driver")
        if (IsOwner)
        {
            Debug.Log("I am owner");
        }
        if (!IsOwner)
        {
            Debug.Log("not owner");
        }
        if (IsOwnedByServer)
        {
            Debug.Log("Server is owner");
        }

        if (!IsClient)
        {
            Debug.Log("inside !IsClient");
        }
        if (!IsServer)
            Debug.Log("inside !IsServer");
        if (IsLocalPlayer)
        {
            Debug.Log("inside IsLocalPlayer");
            HandleMovement();
        }
        */
        if (PlayerRoleString() == "Driver")
        {
            //HandleMovement();
            HandleMovementServerAuth();
        }

        else if (PlayerRoleString() == "Shooter")
        {
            HandleTowerRotationServerAuth();
            //HandleTowerRotation();
            //HandleShooting();
        }
    }



    private void HandleMovement()
    {
        Vector2 inputVector = inputHandler.MoveInput;
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = _moveSpeed * Time.deltaTime;
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


    private void HandleMovementServerAuth()
    {
        Vector2 inputVector = inputHandler.MoveInput;
        HandleMovementServerRpc(inputVector);
    }

    [ServerRpc(RequireOwnership =false)]
    private void HandleMovementServerRpc(Vector2 inputVector)
    {
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = _moveSpeed * Time.deltaTime;
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

    private void HandleTowerRotation()
    {
        // Get mousePosition in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

        // Get mousePosition on X-Z plane
        Vector3 mousePositionHorizontal = new Vector3(mousePosition.x, _tower.position.y, mousePosition.z);

        // Get direction between tower position and mouse position
        Vector3 directionToMouse = mousePositionHorizontal - _tower.position;

        // Apply rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse, Vector3.up);
        _tower.rotation = Quaternion.Slerp(_tower.rotation, targetRotation, _towerRotationSpeed * Time.deltaTime);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 inputVector = inputHandler.MoveInput;
        float playerRadius = 0.7f;
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        Gizmos.DrawWireCube(transform.position + moveDir, new Vector3(playerRadius, playerRadius, playerRadius));

        Ray ray = new Ray(_tower.position, _tower.transform.forward);
        Gizmos.DrawRay(ray.origin, ray.direction * 10f);
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
            return "ERROR";
    }

}
