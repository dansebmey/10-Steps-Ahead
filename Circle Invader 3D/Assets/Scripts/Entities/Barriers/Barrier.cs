using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : OrbitingObject
{
    private BarrierManager Bm => Gm.BarrierManager;
    private Material _material;
    private ParticleSystem _healingParticles;
    private ParticleSystem _damageParticles;

    private int _health;

    private bool _isCollapsed;
    public bool IsCollapsed
    {
        get => _isCollapsed;
        set
        {
            _isCollapsed = value;
            targetPos = _isCollapsed ?
                new Vector3(targetPos.x, -0.8f, targetPos.z)
                : new Vector3(targetPos.x, 0, targetPos.z);
        }
    }

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
                _material.color = Bm.healthColours[0];
                IsCollapsed = true;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _material = GetComponentInChildren<MeshRenderer>().material;
        _healingParticles = GetComponentsInChildren<ParticleSystem>()[0];
        _damageParticles = GetComponentsInChildren<ParticleSystem>()[1];
        
        Health = Bm.initBarrierHealth;
    }

    public int TakeDamage(int amount)
    {
        int cachedHealth = Health;
        Health -= amount;
        
        Gm.LowPassFilterManager.UpdateLPFilter();
        
        transform.position = new Vector3(
            (distanceFromCenter + 0.15f) * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex),
                transform.position.y,
            (distanceFromCenter + 0.15f) * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex));
        _damageParticles.Play();
        
        Gm.BarrierManager.UpdateRemainingBarrierHealth(false);

        int lostHP = cachedHealth - Health;
        Gm.BarrierManager.damageIndicator.Trigger(-lostHP);
        
        return lostHP;
    }

    public void RestoreHealth(int amount)
    {
        int cachedHealth = Health;
        Health += amount;
        
        _healingParticles.Play();
            
        Gm.LowPassFilterManager.UpdateLPFilter();
        Gm.BarrierManager.UpdateRemainingBarrierHealth(true);
        
        int restoredHP = Health - cachedHealth;
        Gm.DamageMitigated += restoredHP;
        Gm.BarrierManager.damageIndicator.Trigger(restoredHP);
    }
}
