﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayerActionState : State
{
    private Dictionary<KeyCode, Func<bool>> _inputMap;

    public override void Init(GameManager gm)
    {
        base.Init(gm);
        _inputMap = new Dictionary<KeyCode, Func<bool>>
        {
            {KeyCode.A, MovePlayerLeft},
            {KeyCode.D, MovePlayerRight},
            {KeyCode.Space, ConsumeSelectedItem},
            {KeyCode.W, SelectNextItem},
            {KeyCode.S, SelectPreviousItem},
            {KeyCode.Escape, ShowSettingsOverlay}
        };
    }

    private bool MovePlayerLeft()
    {
        Gm.CurrentPosIndex--;
        return true;
    }

    private bool MovePlayerRight()
    {
        Gm.CurrentPosIndex++;
        return true;
    }

    private bool ConsumeSelectedItem()
    {
        return Gm.player.Inventory.ConsumeSelectedItem();
    }

    private bool SelectNextItem()
    {
        if (Gm.player.Inventory.carriedPowerups.Count >= 2)
        {
            Gm.player.Inventory.SelectNextPowerup();
            return true;
        }

        return false;
    }

    private bool SelectPreviousItem()
    {
        if (Gm.player.Inventory.carriedPowerups.Count >= 2)
        {
            Gm.player.Inventory.SelectPreviousPowerup();
            return true;
        }
        
        return false;
    }

    public bool ShowSettingsOverlay()
    {
        Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.SettingsInGame);
        return false;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        foreach (KeyValuePair<KeyCode, Func<bool>> entry in _inputMap)
        {
            if (Input.GetKeyDown(entry.Key))
            {
                if (entry.Value.Invoke())
                {
                    Gm.OnPlayerCommandPerformed(entry.Key);
                }
            }
        }
    }

    public override void OnExit()
    {
        
    }
}