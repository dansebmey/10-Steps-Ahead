using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : GmAwareObject
{
    [Range(1,5)] public int maxCapacity;
    [HideInInspector] public List<Powerup> carriedPowerups;
    
    private InventoryInterface _invInterface;

    private int _highlightedItemIndex;

    protected override void Awake()
    {
        base.Awake();
        
        _invInterface = FindObjectOfType<InventoryInterface>(true);
        carriedPowerups = new List<Powerup>();
    }

    public bool AddPowerup(Powerup powerup)
    {
        Gm.TutorialManager.ShowInstruction(
            TutorialManager.InstructionEnum.ItemUsing,
            KeyCode.Space);
        
        if (carriedPowerups.Count < maxCapacity)
        {
            carriedPowerups.Add(powerup);
            _invInterface.UpdateItemSlots();
        
            if (carriedPowerups.Count >= 2 && !Gm.BarrierManager.IsBarrierCollapsed(Gm.CurrentPosIndex)
                                           && Gm.OverlayManager.Hud.IsNoOtherInstructionShown())
            {
                Gm.TutorialManager.ShowInstruction(
                    TutorialManager.InstructionEnum.ItemSwapping,
                    KeyCode.W, KeyCode.S);
            }
            return true;
        }

        return false;
    }

    public void SelectNextPowerup()
    {
        HighlightedItemIndex++;
    }

    public void SelectPreviousPowerup()
    {
        HighlightedItemIndex--;
    }

    public int HighlightedItemIndex
    {
        get => _highlightedItemIndex;
        set
        {
            if (value < 0)
            {
                value += maxCapacity;
            }
            else if (value >= maxCapacity)
            {
                value -= maxCapacity;
            }
            _highlightedItemIndex = value;

            _invInterface.HighlightItem(_highlightedItemIndex);
        }
    }

    public bool ConsumeSelectedItem()
    {
        if (carriedPowerups.Count == 0) return false;
        
        carriedPowerups[_highlightedItemIndex].OnConsume();
        carriedPowerups.Remove(carriedPowerups[_highlightedItemIndex]);
        _invInterface.UpdateItemSlots();
        return true;
    }

    public void Flush()
    {
        carriedPowerups = new List<Powerup>();
        _invInterface.UpdateItemSlots();
    }

    public void LoadFromSaveData(PlayerData playerData)
    {
        foreach (string powerupName in playerData.inventory.carriedPowerupNames)
        {
            Powerup powerup = Gm.FieldItemManager.FindPowerupByName(powerupName);
        
            carriedPowerups.Add(powerup);
            _invInterface.UpdateItemSlots();
        }

        HighlightedItemIndex = playerData.inventory.highlightedItemIndex;
    }

    public bool Contains(string powerupName)
    {
        return carriedPowerups.Any(powerup => powerup.powerupName == powerupName);
    }
}
