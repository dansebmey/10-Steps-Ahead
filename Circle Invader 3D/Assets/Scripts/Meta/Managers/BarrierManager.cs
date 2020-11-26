using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierManager : MonoBehaviour
{
    [SerializeField] private Barrier barrierPrefab;
    [Range(2,32)] public int amountOfBarriers;

    private GameManager _gameManager;
    private Barrier[] _barriers;

    private void Awake()
    {
        _gameManager = GetComponentInParent<GameManager>();
    }

    private void Start()
    {
        InitialiseBarriers();
    }

    private void InitialiseBarriers()
    {
        _barriers = new Barrier[amountOfBarriers];
        for (int i = 0; i < amountOfBarriers; i++)
        {
            Vector3 pos = new Vector3(
                3 * Mathf.Cos((Mathf.PI * 2 / amountOfBarriers) * i),
                0, 
                3 * Mathf.Sin((Mathf.PI * 2 / amountOfBarriers) * i));
            Quaternion rot = Quaternion.LookRotation(transform.position - pos);

            var bar = Instantiate(barrierPrefab, pos, rot);
            bar.PositionIndex = i;
            bar.transform.parent = transform;
            _barriers[i] = bar;
        }
    }

    public void DamageBarrier(int damageDealt, int positionIndex = -1)
    {
        if (positionIndex == -1)
        {
            positionIndex = _gameManager.currentPositionIndex;
        }
        foreach (Barrier bar in _barriers)
        {
            if (barrierPrefab.PositionIndex % 20 == positionIndex)
            {
                
            }
        }
    }
}
