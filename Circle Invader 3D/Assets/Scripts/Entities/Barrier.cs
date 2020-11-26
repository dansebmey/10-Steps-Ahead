using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private int _health;
    [SerializeField] private int initHealth = 3;
    [SerializeField] private int maxHealth = 4;

    public int PositionIndex { get; set; }

    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, maxHealth);
        }
    }

    private Material _material;
    [SerializeField] private Color[] healthColours;

    private void Awake()
    {
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    void Start()
    {
        _health = initHealth;
    }

    public void TakeDamage(int amount)
    {
        _health -= amount;
        
        _material.color = healthColours[_health-1];
    }
}
