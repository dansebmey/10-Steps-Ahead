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
    
    private ConcurrentQueue<FieldItem> _powerupsInGame;

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

        _powerupsInGame = new ConcurrentQueue<FieldItem>();
    }

    public void OnPlayerCommandPerformed()
    {
        _stepsSinceLastItemSpawn++;
        DeterminePowerupSpawn();
        ReducePowerupTimers();
    }

    private void DeterminePowerupSpawn()
    {
        if (_powerupsInGame.Count == 0 && EligibleForPowerupSpawn())
        {
            SpawnItem(SelectRandomItemToSpawn(Random.Range(0, itemPrefabs.Count)));
        }
    }

    private FieldItem SelectRandomItemToSpawn(int rn)
    {
        FieldItem item = itemPrefabs[rn];
        if (Gm.IsScoreHigherThan(item.scoreReq))
        {
            Debug.Log("Spawning item ["+item+"] with score req ["+item.scoreReq+"]");
            return item;
        }

        return SelectRandomItemToSpawn((rn + 1) % (itemPrefabs.Count - 1));
    }

    private void SpawnItem(FieldItem itemPrefab)
    {
        _stepsSinceLastItemSpawn = 0;
        
        FieldItem fieldItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        fieldItem.CurrentPosIndex = Random.Range(0, Gm.BarrierManager.amountOfBarriers-1);
        fieldItem.transform.position = fieldItem.targetPos;
        
        Gm.AudioManager.Play("ItemSpawn", 0.05f);
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
        foreach (FieldItem powerup in _powerupsInGame)
        {
            powerup.ReduceTimer();
        }
    }

    public void HandlePowerupCheck()
    {
        foreach (FieldItem item in _powerupsInGame)
        {
            if (item.CurrentPosIndex % Gm.BarrierManager.amountOfBarriers == Gm.CurrentPosIndex)
            {
                Gm.AudioManager.Play("Collect", 0.05f);
                item.OnPickup();
            }
        }
    }

    public void RegisterPowerup(FieldItem item)
    {
        _powerupsInGame.Enqueue(item);
    }

    public void DeleteItem(FieldItem item)
    {
        _powerupsInGame.TryDequeue(out item);
        // TODO: This bit of code (and the fact that I'm using a ConcurrentBag) hinders multiple items from being
        // TODO: in the game simultaneously
        _stepsSinceLastItemSpawn = 0;
    }
}
