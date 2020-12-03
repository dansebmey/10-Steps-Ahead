using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerupManager : GmAwareObject, IPlayerCommandListener
{
    [SerializeField] private List<FieldItem> powerupPrefabs;
    
    private ConcurrentBag<FieldItem> _powerupsInGame;
    private object _powerupListLock = new object();

    protected override void Awake()
    {
        base.Awake();

        lock (_powerupListLock)
        {
            _powerupsInGame = new ConcurrentBag<FieldItem>();
        }
    }

    public void OnPlayerCommandPerformed()
    {
        DeterminePowerupSpawn();
    }

    private void DeterminePowerupSpawn()
    {
        lock (_powerupListLock)
        {
            if (_powerupsInGame.Count == 0)
            {
                FieldItem fieldItem = Instantiate(powerupPrefabs[0], Vector3.zero, Quaternion.identity);
                fieldItem.CurrentPosIndex = Random.Range(0, Gm.BarrierManager.amountOfBarriers-1);
                fieldItem.transform.position = fieldItem.targetPos;
            }
        }
    }

    public void HandlePowerupCheck()
    {
        lock (_powerupListLock)
        {
            foreach (FieldItem powerup in _powerupsInGame)
            {
                if (powerup.CurrentPosIndex % Gm.BarrierManager.amountOfBarriers == Gm.CurrentPosIndex)
                {
                    powerup.OnPickup();
                }
            }
        }
    }

    public void RegisterPowerup(FieldItem item)
    {
        lock (_powerupListLock)
        {
            _powerupsInGame.Add(item);
        }
    }

    public void DeletePowerup(FieldItem item)
    {
        lock (_powerupListLock)
        {
            _powerupsInGame.TryTake(out item);
        }
    }
}
