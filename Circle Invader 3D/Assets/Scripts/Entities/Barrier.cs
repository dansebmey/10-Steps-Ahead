using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : OrbitingObject, IDamageable
{
    private BarrierManager Bm => Gm.BarrierManager;
    private Material _material;
    private ParticleSystem _healingParticles;
    private ParticleSystem _damageParticles;

    private int _health;
    private int _remainingCollapsedTurns;

    // public float offsetCounter;
    // private float _yOffset;

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, Bm.maxBarrierHealth);
            if (_health > 0)
            {
                _material.color = Bm.healthColours[Health-1];
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
            RemainingCollapsedTurns = Bm.collapsedTurnCount;
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
        Health = Bm.initBarrierHealth;
    }

    // protected override void Update()
    // {
    //     base.Update();
    //     // if (transform.position != targetPos)
    //     // {
    //     //     transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
    //     // }
    //     // offsetCounter += 0.0625f;
    //     // _yOffset = 0.01f + (0.01f * Mathf.Sin(offsetCounter + ((Mathf.PI * 2 / _bm.amountOfBarriers) * (CurrentPosIndex % 2))));
    //     // transform.position += new Vector3(0, _yOffset, 0);
    // }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        transform.position = new Vector3(
            (distanceFromCenter + 0.35f) * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex),
                transform.position.y,
            (distanceFromCenter + 0.35f) * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex));
        _damageParticles.Play();
    }

    public void RestoreHealth(int amount)
    {
        Health += amount;
        _healingParticles.Play();
    }
}
