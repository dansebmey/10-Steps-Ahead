using System;
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

    public void OnNewGameStart()
    {
        foreach (FieldItem item in ItemsInField)
        {
            item.Destroy();
        }
        ItemsInField = new ConcurrentQueue<FieldItem>();
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
        if (ItemsInField.Count == 0 && EligibleForPowerupSpawn())
        {
            SpawnItem(SelectRandomItemToSpawn());
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

        List<Barrier> eligibleBarriers = Gm.BarrierManager.Barriers.Where(b => !b.IsCollapsed && b.CurrentPosIndex != Gm.player.CurrentPosIndex).ToList();
        fieldItem.CurrentPosIndex = eligibleBarriers[Random.Range(0, eligibleBarriers.Count)].CurrentPosIndex;
        
        fieldItem.transform.position = fieldItem.targetPos;
        
        Gm.AudioManager.Play("ItemSpawn");
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
}