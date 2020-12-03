using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour, IPlayerCommandListener
{
    public void OnPlayerCommandPerformed()
    {
        DeterminePowerupSpawn();
    }

    private void DeterminePowerupSpawn()
    {
        
    }
}
