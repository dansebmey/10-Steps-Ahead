using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PowerupManager : GmAwareObject, IPlayerCommandListener
{
    [SerializeField] private List<Powerup> powerupPrefabs;
    private List<Powerup> powerupsInGame;

    protected override void Awake()
    {
        base.Awake();
        powerupsInGame = new List<Powerup>();
    }

    public void OnPlayerCommandPerformed()
    {
        DeterminePowerupSpawn();
    }

    private void DeterminePowerupSpawn()
    {
        if (powerupsInGame.Count == 0)
        {
            Powerup powerup = Instantiate(powerupPrefabs[0], Vector3.zero, Quaternion.identity);
            powerup.CurrentPosIndex = Random.Range(0, Gm.BarrierManager.amountOfBarriers-1);
            powerup.transform.position = powerup.targetPos;
        }
    }

    public void HandlePowerupCheck()
    {
        foreach (Powerup powerup in powerupsInGame)
        {
            if (powerup.CurrentPosIndex % Gm.BarrierManager.amountOfBarriers == Gm.CurrentPosIndex)
            {
                powerup.OnPickup();
            }
        }
    }

    public void RegisterPowerup(Powerup powerup)
    {
        powerupsInGame.Add(powerup);
    }
}
