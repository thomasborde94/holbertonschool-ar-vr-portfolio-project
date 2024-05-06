using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class Enemy : NetworkBehaviour
{
    [SerializeField] protected Animator _anim;

    [SerializeField] protected float _speed;
    [SerializeField] protected float _rangeToAttack;
    [SerializeField] protected float _rangeToStopMoving;
    [Header("Coins Drop")]
    [SerializeField] GameObject _coinPrefab;
    [SerializeField] int _coinsAmountDropped = 5;
    [SerializeField] float _coinSpawnRadius = 1.5f;

    protected Transform _transform;
    public int index;

    protected bool isHit = false;
    protected bool canMove = true;
    protected Transform _player;
    public bool isDead = false;
    public float _maxHealth = 10;
    [SerializeField] public NetworkVariable<int> _currentHealth = new NetworkVariable<int>();

    private float resetTriggerTime = 0.5f;
    private bool droppedCoin = false;
    private List<GameObject> droppedCoins = new List<GameObject>();
    public float distance;

    #region Unity Lifecycle

    private void Start()
    {
        if (!IsServer) return;
        Init();
    }

    public override void OnNetworkSpawn()
    {
        Init();
        base.OnNetworkSpawn();
    }



    public virtual void Init()
    {
        _transform = GetComponent<Transform>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (IsServer)
            _currentHealth.Value = 10 * EnemySpawner.Instance.currentRound * 2;
    }

    public virtual void Update()
    {
        if (!isDead)
        {
            Movement();
        }
        else
            TriggerDeathServerRpc();

    }
    #endregion

    public virtual void Movement()
    {
        if (_player != null)
        {
            _transform.LookAt(new Vector3(_player.localPosition.x, 0, _player.localPosition.z));
            distance = Vector3.Distance(_transform.position, _player.transform.position);
            if (CanMove())
            {
                _transform.position = Vector3.MoveTowards(_transform.localPosition, new Vector3(_player.localPosition.x, 0, _player.localPosition.z),
                    _speed * Time.deltaTime);
            }


            if (distance > _rangeToAttack)
            {
                _anim.SetBool("Move", true);
            }
            else
            {
                _anim.SetTrigger("Attack");
                if (IsInRangeToAttack())
                    _anim.SetBool("IsInRangeToAttack", true);
                else
                    _anim.SetBool("IsInRangeToAttack", false);

                StartCoroutine(ResetAttackTrigger(resetTriggerTime));


            }
        }
        else
            Debug.Log("cant find player");

    }

    private void DropCoins()
    {
        for (int i = 0; i < _coinsAmountDropped; i++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * _coinSpawnRadius;
            Vector3 coinSpawnPosition = transform.position + randomOffset;
            GameObject coin = Instantiate(_coinPrefab, coinSpawnPosition + new Vector3(0, _coinSpawnRadius, 0), Quaternion.identity);
            NetworkObject coinNO = coin.GetComponent<NetworkObject>();
            coinNO.Spawn();
            droppedCoins.Add(coin);

            Rigidbody coinRigidbody = coin.GetComponent<Rigidbody>();
            if (coinRigidbody != null)
            {
                coinRigidbody.AddForce(UnityEngine.Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
        droppedCoin = true;
        StartCoroutine(GatherCoins());
    }

    private IEnumerator GatherCoins()
    {
        yield return new WaitForSeconds(1f);
        float lerpTime = 4f; // Temps en secondes pour que les pièces atteignent le joueur
        float elapsedTime = 0f;

        while (elapsedTime < lerpTime)
        {
            elapsedTime += Time.deltaTime;

            foreach (GameObject coin in droppedCoins)
            {
                if (coin != null)
                {
                    Vector3 currentPosition = coin.transform.position;
                    coin.transform.position = Vector3.Lerp(currentPosition, _player.position, elapsedTime / lerpTime);
                }
            }

            yield return null;
        }

        droppedCoins.Clear();
    }
    private IEnumerator ResetAttackTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        _anim.ResetTrigger("Attack");
    }

    #region RPCs
    [ServerRpc(RequireOwnership = false)]
    private void TriggerDeathServerRpc()
    {
        _anim.SetBool("Dead", true);
        Destroy(gameObject, 3f);
        if (!droppedCoin)
        {
            DropCoins();
        }
            
    }


    [ServerRpc(RequireOwnership = false)]
    public void GotHitServerRpc()
    {
        if (_currentHealth.Value <= 0)
            SetIsDeadClientRpc(true);

        else
            _anim.SetTrigger("Hit");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCurrentHealthLossServerRpc(int healthAmountLost, ServerRpcParams serverRpcParams = default)
    {
        _currentHealth.Value -= healthAmountLost;
    }
    [ClientRpc]
    public void SetIsDeadClientRpc(bool value, ClientRpcParams clientRpcParams = default)
    {
        isDead = value;
    }
    #endregion

    #region Public properties
    public bool CanMove()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("battleidle") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("attack1withhitbox") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("hit"))
            return false;
        if (distance < _rangeToStopMoving)
            return false;
        else
            return true;
    }

    public bool IsInRangeToAttack()
    {
        if (distance < _rangeToStopMoving)
            return true;
        else
            return false;
    }

    public float GetCurrentHealthPart()
    {
        return (_currentHealth.Value / _maxHealth);
    }

    #endregion
}
