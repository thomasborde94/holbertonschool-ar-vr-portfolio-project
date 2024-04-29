using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Wolfboss :Enemy
{
    public int Health { get; private set; }

    void Start()
    {
        if (IsServer)
            Init();
    }

    public override void Movement()
    {
        base.Movement();
    }

    public override void Init()
    {
        base.Init();
        if (IsServer)
            _currentHealth.Value = 5;
    }

    public override void Update()
    {
        base.Update();
        Debug.Log(_currentHealth.Value);
    }
}
