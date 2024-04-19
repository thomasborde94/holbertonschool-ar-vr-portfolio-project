using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Watcher_B : Enemy
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
}
