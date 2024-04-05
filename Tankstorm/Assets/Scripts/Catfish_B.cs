using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catfish_B : Enemy
{
    public int Health { get; private set; }
    void Start()
    {
        Init();
        Health = base._health;
    }

    public override void Movement()
    {
        base.Movement();
    }
}
