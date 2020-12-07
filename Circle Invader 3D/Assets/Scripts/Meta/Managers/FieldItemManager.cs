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

    [Header("Big Hammer")]
    [SerializeField][Range(3,10)] private int coinsForBigHammer;
    [SerializeField] private FieldItem bigHammerPrefab;
    private int _coinsCollected;

    public int CoinsCollected
    {
        get => _coinsCollected;
        set
        {
            _coinsCollected = value;
            if (_coinsCollected >= coinsForBigHammer)
            {
                _coinsCollected = 0;
                SpawnItem(bigHammerPrefab);
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
                SpawnItem(itemPrefabs[Random.Range(0, itemPrefabs.Count)]);
            }
        }
    }

    private void SpawnItem(FieldItem itemPrefab)
    {
        _stepsSinceLastItemSpawn = 0;
        
        FieldItem fieldItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        fieldItem.CurrentPosIndex = Random.Range(0, Gm.BarrierManager.amountOfBarriers-1);
        fieldItem.transform.position = fieldItem.targetPos;
    }

    private bool EligibleForPowerupSpawn()
    {
        float chance = -((1.0f / stepsUntilGuaranteedSpawn) * (stepsUntilGuaranteedSpawn * 0.5f))
                       + ((1.0f / stepsUntilGuaranteedSpawn) * _stepsSinceLastItemSpawn);
        float rn = Random.Range(0.0f, 1.0f);
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
            // TODO: This bit of code (and the fact that I'm using a ConcurrentBag) hinders multiple items from being
            // TODO: in the game simultaneously 
        }
        _stepsSinceLastItemSpawn = 0;
    }
}
