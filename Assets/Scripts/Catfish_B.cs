using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Catfish_B : Enemy
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
        if (IsServer)
            _currentHealth.Value = 15 * (int)Math.Pow(EnemySpawner.Instance.currentRound, EnemySpawner.Instance.currentRound);
    }
}
