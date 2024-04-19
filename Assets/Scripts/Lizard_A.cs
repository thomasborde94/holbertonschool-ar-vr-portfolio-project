using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lizard_A : Enemy
{
    public int Health {  get; private set; }
    void Start()
    {
        if (IsServer)
            Init();
        //Health = base._health;
    }

    public override void Init()
    {
        base.Init();
        _currentHealth.Value = 15 * (int)Math.Pow(EnemySpawner.Instance.currentRound, EnemySpawner.Instance.currentRound);
    }

    public override void Movement()
    {
        base.Movement();
    }
}

