﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class FieldItemManager : GmAwareObject, IPlayerCommandListener, IResetOnGameStart
{
    public int itemLifespanInSteps = 10;
    [SerializeField] private List<FieldItem> itemPrefabs;
    
    public ConcurrentQueue<FieldItem> ItemsInField { get; private set; }

    [SerializeField] private int minStepsUntilSpawn = 8;
    [SerializeField] private int maxStepsUntilSpawn = 16;
    public int StepsSinceLastItemSpawn { get; private set; }

    [Header("Big Hammer")]
    [SerializeField][Range(1,10)] private int coinsForBigHammer;

    public int CoinsForBigHammer => coinsForBigHammer;
    [SerializeField] private FieldItem bigHammerPrefab;
    private int _coinsCollected;

    public int CoinsCollected
    {
        get => _coinsCollected;
        set
        {
            _coinsCollected = value;
            Gm.OverlayManager.Hud.UpdateBigHammerInterface();
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

        ItemsInField = new ConcurrentQueue<FieldItem>();
    }

    public void OnGameReset()
    {
        foreach (FieldItem item in ItemsInField)
        {
            Destroy(item.gameObject);
        }
        ItemsInField = new ConcurrentQueue<FieldItem>();
        CoinsCollected = 0;
        StepsSinceLastItemSpawn = 0;
    }

    public void OnPlayerCommandPerformed()
    {
        StepsSinceLastItemSpawn++;
        DeterminePowerupSpawn();
        ReducePowerupTimers();
    }

    private void DeterminePowerupSpawn()
    {
        if (ItemsInField.Count == 0 && EligibleForPowerupSpawn())
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
        StepsSinceLastItemSpawn = 0;
        
        FieldItem fieldItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        fieldItem.CurrentPosIndex = Random.Range(0, Gm.BarrierManager.amountOfBarriers-1);
        fieldItem.transform.position = fieldItem.targetPos;
        
        Gm.AudioManager.Play("ItemSpawn", 0.05f);
    }

    private bool EligibleForPowerupSpawn()
    {
        float healthMissingPercentage = 
            1 - (1.0f / Gm.BarrierManager.InitBarrierHealth) * Gm.BarrierManager.CurrentBarrierHealth;

        float chance = Mathf.Clamp(1 -((1.0f / minStepsUntilSpawn) * (minStepsUntilSpawn))
                       + ((1.0f / (maxStepsUntilSpawn - minStepsUntilSpawn)) * (StepsSinceLastItemSpawn - minStepsUntilSpawn) * (1.0f + (healthMissingPercentage * 0.5f))), 0, 1);

        float rn = Random.Range(0.0f, 1.0f);
        return rn <= chance;
    }

    private void ReducePowerupTimers()
    {
        foreach (FieldItem powerup in ItemsInField)
        {
            powerup.ReduceTimer();
        }
    }

    public void HandlePowerupCheck()
    {
        foreach (FieldItem item in ItemsInField)
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
        ItemsInField.Enqueue(item);
    }

    public void DeleteItem(FieldItem item)
    {
        ItemsInField.TryDequeue(out item);
        // TODO: This bit of code (and the fact that I'm using a ConcurrentBag) hinders multiple items from being
        // TODO: in the game simultaneously
        StepsSinceLastItemSpawn = 0;
    }
}
