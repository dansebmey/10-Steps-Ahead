using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : OrbitingObject, IDamageable
{
    private BarrierManager _bm => Gm.BarrierManager;
    private Material _material;
    private ParticleSystem _healingParticles;
    private ParticleSystem _damageParticles;

    private int _health;
    private int _remainingCollapsedTurns;

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, _bm.maxBarrierHealth);
            if (_health > 0)
            {
                _material.color = _bm.healthColours[Health-1];
            }
            else
            {
                if (RemainingCollapsedTurns <= 0)
                {
                    ToggleCollapsedState(true);
                }
            }
        }
    }

    public int RemainingCollapsedTurns
    {
        get => _remainingCollapsedTurns;
        set
        {
            _remainingCollapsedTurns = value;
            if (value <= 0)
            {
                ToggleCollapsedState(false);
            }
        }
    }

    private void ToggleCollapsedState(bool doCollapse)
    {
        if (doCollapse)
        {
            RemainingCollapsedTurns = _bm.collapsedTurnCount;
            targetPos = new Vector3(transform.position.x, -0.8f, transform.position.z);
        }
        else
        {
            targetPos = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    public bool IsCollapsed()
    {
        return RemainingCollapsedTurns > 0;
    }

    protected override void Awake()
    {
        base.Awake();
        _material = GetComponentInChildren<MeshRenderer>().material;
        _healingParticles = GetComponentsInChildren<ParticleSystem>()[0];
        _damageParticles = GetComponentsInChildren<ParticleSystem>()[1];
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
        _damageParticles.Play();
    }

    public void RestoreHealth(int amount)
    {
        Health += amount;
        _healingParticles.Play();
    }
}
