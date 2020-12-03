using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : CIObject, IDamageable
{
    [HideInInspector] public BarrierManager bm;
    private Material _material;

    private int _health;
    private int _remainingDormantTurns;

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, bm.maxBarrierHealth);
            if (value > 0)
            {
                _material.color = bm.healthColours[Health-1];
            }
            else
            {
                if (RemainingDormantTurns <= 0)
                {
                    ToggleDormantState(true);
                }
            }
        }
    }

    public int PositionIndex { get; set; }

    public int RemainingDormantTurns
    {
        get => _remainingDormantTurns;
        set
        {
            _remainingDormantTurns = value;
            if (value == 0)
            {
                ToggleDormantState(false);
            }
        }
    }

    private void ToggleDormantState(bool isDormant)
    {
        if (isDormant)
        {
            RemainingDormantTurns = bm.dormantTurnCount;
            targetPos = new Vector3(transform.position.x, -0.8f, transform.position.z);
        }
        else
        {
            Health = 1;
            targetPos = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    public bool IsDormant()
    {
        return RemainingDormantTurns > 0;
    }

    protected override void Awake()
    {
        base.Awake();
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    protected override void Start()
    {
        base.Start();
        Health = bm.initBarrierHealth;
    }

    protected override void Update()
    {
        base.Update();
        if (transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
        }
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
    }
}
