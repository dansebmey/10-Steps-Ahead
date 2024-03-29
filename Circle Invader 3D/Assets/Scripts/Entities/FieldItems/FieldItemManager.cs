﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class FieldItemManager : GmAwareObject, IPlayerCommandListener, IResetOnGameStart
{
    public int itemLifespanInSteps = 10;
    [SerializeField] private List<FieldItem> itemPrefabs;
    
    public List<FieldItem> ItemsInField { get; private set; }

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

        ItemsInField = new List<FieldItem>();
    }

    public void OnNewGameStart()
    {
        foreach (FieldItem item in ItemsInField)
        {
            item.Destroy();
            ItemsInField.Remove(item);
        }
        ItemsInField = new List<FieldItem>();
        CoinsCollected = 0;
        StepsSinceLastItemSpawn = 0;
    }

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        StepsSinceLastItemSpawn++;
        DeterminePowerupSpawn();
        ReducePowerupTimers();
    }

    private void DeterminePowerupSpawn()
    {
        // TODO: Warp animation?
        
        if (ItemsInField.Where(i => !(i.item is Mine)).ToList().Count == 0
            && EligibleForPowerupSpawn()
            && Gm.BarrierManager.Barriers.Where(b => !b.IsCollapsed).ToList().Count > 1)
        {
            FieldItem item = SelectRandomItemToSpawn();
            SpawnItem(item);
        }
    }

    private FieldItem SelectRandomItemToSpawn()
    {
        List<FieldItem> fieldItems = DetermineAvailableItems();
        fieldItems.Sort(new ItemSorter());
        
        int totalWeight = CalculateTotalWeight(fieldItems);
        int cumulativeWeight = 0;
        
        int rn = Random.Range(0, totalWeight);
        foreach (FieldItem fieldItem in fieldItems)
        {
            cumulativeWeight += fieldItem.spawnWeight;
            if (rn <= cumulativeWeight)
            {
                return fieldItem;
            }
        }

        return null;
    }

    private int CalculateTotalWeight(List<FieldItem> fieldItems)
    {
        int total = 0;
        foreach (FieldItem fieldItem in fieldItems)
        {
            total += fieldItem.spawnWeight;
        }

        return total;
    }

    private List<FieldItem> DetermineAvailableItems()
    {
        List<FieldItem> result = new List<FieldItem>();
        foreach (FieldItem fieldItem in itemPrefabs)
        {
            if (fieldItem.spawnWeight > 0 && Gm.IsScoreHigherThan(fieldItem.scoreReq))
            {
                result.Add(fieldItem);
            }
        }

        return result;
    }

    private void SpawnItem(FieldItem itemPrefab)
    {
        StepsSinceLastItemSpawn = 0;
        
        FieldItem fieldItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);

        List<Barrier> eligibleBarriers = Gm.BarrierManager.Barriers.Where(b => !b.IsCollapsed && !IsBarrierOccupied(b.CurrentPosIndex)).ToList();
        fieldItem.CurrentPosIndex = eligibleBarriers[Random.Range(0, eligibleBarriers.Count)].CurrentPosIndex;
        
        fieldItem.transform.position = fieldItem.targetPos;
        
        Gm.AudioManager.Play("ItemSpawn");
    }

    public bool IsBarrierOccupied(int barrierPosIndex)
    {
        return barrierPosIndex == Gm.player.CurrentPosIndex || ItemsInField.Any(i => i.CurrentPosIndex == barrierPosIndex);
    }

    private bool EligibleForPowerupSpawn()
    {
        float chance = Mathf.Clamp(1 -((1.0f / minStepsUntilSpawn) * (minStepsUntilSpawn))
                                   + ((1.0f / (maxStepsUntilSpawn - minStepsUntilSpawn)) * (StepsSinceLastItemSpawn - minStepsUntilSpawn)), 0, 1);

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
                item.OnPickup();
            }
        }
    }

    public void RegisterPowerup(FieldItem item)
    {
        ItemsInField.Add(item);
    }

    public void DeleteItem(FieldItem itemToRemove)
    {
        List<FieldItem> newList = ItemsInField.Where(i => i != itemToRemove).ToList();
        ItemsInField = newList;
        
        StepsSinceLastItemSpawn = 0;
    }
    
    #region OnGameLoad

    public void OnGameLoad(GameData gameData)
    {
        FieldItemManagerData fimData = gameData.fimData;
        CoinsCollected = fimData.coinsCollected;
        foreach (FieldItemManagerData.FieldItemData itemData in fimData.fieldItems)
        {
            LoadItemFromSaveData(itemData);
        }
    }

    private void LoadItemFromSaveData(FieldItemManagerData.FieldItemData itemData)
    {
        FieldItem item = FindItemPrefabByName(itemData.itemName);
        int posIndex = itemData.posIndex;
        int remainingDuration = itemData.remainingDuration;
        
        FieldItem fieldItem = Instantiate(item, Vector3.zero, Quaternion.identity);
        fieldItem.CurrentPosIndex = posIndex;
        fieldItem.transform.position = fieldItem.targetPos;
        fieldItem.RemainingDuration = remainingDuration;
    }

    private FieldItem FindItemPrefabByName(string itemName)
    {
        foreach (FieldItem itemPrefab in itemPrefabs)
        {
            if (itemPrefab.item.GetType().ToString() == itemName)
            {
                return itemPrefab;
            }
        }

        Debug.LogError("No item found with name ["+itemName+"]");
        return null;
    }
    
    #endregion

    public Powerup FindPowerupByName(string powerupName)
    {
        foreach (FieldItem itemPrefab in itemPrefabs)
        {
            if (itemPrefab.item.GetType().ToString() == powerupName)
            {
                return itemPrefab.item as Powerup;
            }
        }

        return null;
    }
    
    class ItemSorter : IComparer<FieldItem> 
    {
        public int Compare(FieldItem a, FieldItem b)
        {
            if (a.spawnWeight < b.spawnWeight)
            {
                return -1;
            }
            return b.spawnWeight > a.spawnWeight ? 1 : 0;
        }
    }

    public int ClearMines()
    {
        int damagePrevented = 0;

        foreach (FieldItem item in ItemsInField)
        {
            if (item.item is Mine)
            {
                item.Destroy();
                damagePrevented++;
            }
        }

        return damagePrevented;
    }
}