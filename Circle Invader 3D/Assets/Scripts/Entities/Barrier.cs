using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : OrbitingObject, IDamageable
{
    private BarrierManager _bm => Gm.BarrierManager;
    private Material _material;
    private ParticleSystem _particleSystem;

    private int _health;
    private int _remainingDormantTurns;

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, _bm.maxBarrierHealth);
            if (value > 0)
            {
                _material.color = _bm.healthColours[Health-1];
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
            RemainingDormantTurns = _bm.dormantTurnCount;
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
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    protected override void Start()
    {
        base.Start();
        Health = _bm.initBarrierHealth;
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

    public void RestoreHealth(int amount)
    {
        Health += amount;
        _particleSystem.Play();
    }
}
