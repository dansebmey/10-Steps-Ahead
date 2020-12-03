using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour, IPlayerCommandListener
{
    [Range(2,32)] public int amountOfBarriers;
    public float barrierDistanceFromCenter = 2.5f;
    
    [Header("Barrier variables")]
    [SerializeField] private Barrier barrierPrefab;
    [Range(1,10)][SerializeField] public int initBarrierHealth = 3;
    [Range(1,10)][SerializeField] public int maxBarrierHealth = 4;
    [SerializeField] public Color[] healthColours;
    [Range(10, 100)] public int dormantTurnCount = 30;

    private Barrier[] _barriers;

    private GameManager _gm;

    private void Awake()
    {
        _gm = GetComponentInParent<GameManager>();
    }

    protected void Start()
    {
        InitialiseBarriers();
    }

    private void InitialiseBarriers()
    {
        _barriers = new Barrier[amountOfBarriers];
        for (int i = 0; i < amountOfBarriers; i++)
        {
            Vector3 pos = new Vector3(
                barrierDistanceFromCenter * Mathf.Cos((Mathf.PI * 2 / amountOfBarriers) * i),
                0, 
                barrierDistanceFromCenter * Mathf.Sin((Mathf.PI * 2 / amountOfBarriers) * i));
            Quaternion rot = Quaternion.LookRotation(transform.position - pos);

            var bar = Instantiate(barrierPrefab, pos, rot);
            bar.CurrentPosIndex = i;
            bar.name = "Barrier (index " + i + ")";
            bar.transform.parent = transform;
            _barriers[i] = bar;
        }
    }

    public void DamageBarrier(int damageDealt, int positionIndex)
    {
        foreach (var bar in _barriers)
        {
            if (bar.CurrentPosIndex % amountOfBarriers == positionIndex)
            {
                bar.TakeDamage(damageDealt);
            }
        }
    }

    public bool IsBarrierDormant(int posIndex)
    {
        foreach (var bar in _barriers)
        {
            if (bar.CurrentPosIndex % amountOfBarriers == posIndex)
            {
                return bar.IsDormant();
            }
        }
        
        Debug.LogError("No barrier was found with posIndex ["+posIndex+"]");
        return false;
    }

    public void OnPlayerCommandPerformed()
    {
        foreach (var bar in _barriers)
        {
            bar.RemainingDormantTurns--;
        }
    }
}
