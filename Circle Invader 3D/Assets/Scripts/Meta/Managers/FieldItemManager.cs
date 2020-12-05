using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class FieldItemManager : GmAwareObject, IPlayerCommandListener
{
    public int itemLifespanInSteps = 10;
    [SerializeField] private List<FieldItem> itemPrefabs;
    
    private ConcurrentBag<FieldItem> _powerupsInGame;
    private object _itemListLock = new object();

    [SerializeField] private int stepsUntilGuaranteedSpawn = 16;
    private int _stepsSinceLastItemSpawn;

    [SerializeField][Range(3,10)] private int coinsForBigHammer = 8;
    private int _coinsCollected;

    public int CoinsCollected
    {
        get => _coinsCollected;
        set
        {
            _coinsCollected = value;
            if (_coinsCollected >= coinsForBigHammer)
            {
                Gm.BarrierManager.RepairAllBarriers(1);
                _coinsCollected = 0;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        lock (_itemListLock)
        {
            _powerupsInGame = new ConcurrentBag<FieldItem>();
        }
    }

    public void OnPlayerCommandPerformed()
    {
        _stepsSinceLastItemSpawn++;
        DeterminePowerupSpawn();
        ReducePowerupTimers();
    }

    private void DeterminePowerupSpawn()
    {
        lock (_itemListLock)
        {
            if (_powerupsInGame.Count == 0 && EligibleForPowerupSpawn())
            {
                _stepsSinceLastItemSpawn = 0;
                
                int randomItemIndex = Random.Range(0, itemPrefabs.Count);
                FieldItem fieldItem = Instantiate(itemPrefabs[randomItemIndex], Vector3.zero, Quaternion.identity);
                fieldItem.CurrentPosIndex = Random.Range(0, Gm.BarrierManager.amountOfBarriers-1);
                fieldItem.transform.position = fieldItem.targetPos;
            }
        }
    }

    private bool EligibleForPowerupSpawn()
    {
        float chance = -((1.0f / stepsUntilGuaranteedSpawn) * (stepsUntilGuaranteedSpawn * 0.5f))
                       + ((1.0f / stepsUntilGuaranteedSpawn) * _stepsSinceLastItemSpawn);
        float rn = Random.Range(0.0f, 1.0f);
        // Debug.Log(rn + " <= " + chance + " = " + (rn <= chance));
        return rn <= chance;
    }

    private void ReducePowerupTimers()
    {
        lock (_itemListLock)
        {
            foreach (FieldItem powerup in _powerupsInGame)
            {
                powerup.ReduceTimer();
            }
        }
    }

    public void HandlePowerupCheck()
    {
        lock (_itemListLock)
        {
            foreach (FieldItem item in _powerupsInGame)
            {
                if (item.CurrentPosIndex % Gm.BarrierManager.amountOfBarriers == Gm.CurrentPosIndex)
                {
                    item.OnPickup();
                }
            }
        }
    }

    public void RegisterPowerup(FieldItem item)
    {
        lock (_itemListLock)
        {
            _powerupsInGame.Add(item);
        }
    }

    public void DeleteItem(FieldItem item)
    {
        lock (_itemListLock)
        {
            _powerupsInGame.TryTake(out item);
            // TODO: This bit of code hinders multiple items from being in the game simultaneously 
        }
        _stepsSinceLastItemSpawn = 0;
    }
}
