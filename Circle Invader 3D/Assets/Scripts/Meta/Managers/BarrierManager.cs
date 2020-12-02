using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    [SerializeField] private Barrier barrierPrefab;
    [Range(2,32)] public int amountOfBarriers;
    [Range(10, 100)] public int dormantTurnCount = 30;

    private Barrier[] _barriers;
    public float distanceFromCenter = 2.5f;

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
                distanceFromCenter * Mathf.Cos((Mathf.PI * 2 / amountOfBarriers) * i),
                0, 
                distanceFromCenter * Mathf.Sin((Mathf.PI * 2 / amountOfBarriers) * i));
            Quaternion rot = Quaternion.LookRotation(transform.position - pos);

            var bar = Instantiate(barrierPrefab, pos, rot);
            bar.PositionIndex = i;
            bar.name = "Barrier (i" + i + ")";
            bar.transform.parent = transform;
            bar.barrierManager = this;
            _barriers[i] = bar;
        }
    }

    public void DamageBarrier(int damageDealt, int positionIndex)
    {
        foreach (Barrier bar in _barriers)
        {
            if (bar.PositionIndex % 20 == positionIndex)
            {
                bar.TakeDamage(damageDealt);
            }
        }
    }
}
