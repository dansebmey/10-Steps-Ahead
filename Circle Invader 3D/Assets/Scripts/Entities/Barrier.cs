using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : CIObject
{
    private int _health;
    [SerializeField] private int initHealth = 3;
    [SerializeField] private int maxHealth = 4;

    public int PositionIndex { get; set; }

    private int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, maxHealth);
            if (value > 0)
            {
                _material.color = healthColours[Health-1];
            }
            else
            {
                if (_remainingDormantTurns == 0)
                {
                    MakeDormant();
                }
            }
        }
    }

    private int _remainingDormantTurns;
    private void MakeDormant()
    {
        _remainingDormantTurns = barrierManager.dormantTurnCount;
        targetPos = new Vector3(transform.position.x, -0.8f, transform.position.z);
    }

    private Material _material;
    [SerializeField] private Color[] healthColours;
    public BarrierManager barrierManager;

    private void Awake()
    {
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    protected override void Start()
    {
        base.Start();
        Health = initHealth;
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
