using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public abstract class Enemy : NetworkBehaviour
{
    [SerializeField] protected Animator _anim;

    [SerializeField] protected int _health;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _rangeToAttack;
    [SerializeField] protected float _rangeToStopMoving;

    protected Transform _transform;
    public int index;

    protected bool isHit = false;
    protected bool canMove = true;
    protected Transform _player;
    public bool isDead = false;
    public float _maxHealth = 10;
    public float _currentHealth = 10;

    private float resetTriggerTime = 0.5f;
    public float distance;

    #region Unity Lifecycle


    private void Start() // ICI ONNETWORK SPAWN
    {
        Init();
    }

    

    public virtual void Init()
    {
        _transform = GetComponent<Transform>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    public virtual void Update()
    {


        if (!isDead)
        {
            Movement();
        }
        else
            TriggerDeath();

    }

    #endregion

    private void TriggerDeath()
    {
        _anim.SetBool("Dead", true);
        Destroy(gameObject, 3f);
    }

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

    private IEnumerator ResetAttackTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        _anim.ResetTrigger("Attack"); //
    }

    public void GotHit()
    {
        if (_currentHealth <= 0)
            isDead = true;

        else
            _anim.SetTrigger("Hit");
    }

    public bool CanMove()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("battleidle") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("attack1withhitbox") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("hit"))
            return false;
        if (distance < _rangeToStopMoving )
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
        return (_currentHealth / _maxHealth);
    }

    public void SetCurrentHealthLoss(float healthAmountLost)
    {
        _currentHealth -= healthAmountLost;
    }

}
