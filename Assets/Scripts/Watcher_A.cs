using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Watcher_A : Enemy
{

    public int Health { get; private set; }
    void Start()
    {
        if (IsServer)
            Init();
    }

    public override void Init()
    {
        base.Init();
        _currentHealth.Value = 10 * (int)Math.Pow(EnemySpawner.Instance.currentRound, EnemySpawner.Instance.currentRound);
    }

    public override void Movement()
    {
        base.Movement();
    }

    public new bool CanMove()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("battleidle") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("attack2withhitbox") ||
                _anim.GetCurrentAnimatorStateInfo(0).IsName("hit"))
            return false;
        if (distance < _rangeToStopMoving)
            return false;
        else
            return true;
    }
}
