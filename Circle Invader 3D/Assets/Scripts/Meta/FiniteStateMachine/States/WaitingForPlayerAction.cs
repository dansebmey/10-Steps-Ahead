using UnityEngine;

public class WaitingForPlayerAction : State
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Gm.CurrentPosIndex--;
            Gm.OnPlayerCommandPerformed();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Gm.CurrentPosIndex++;
            Gm.OnPlayerCommandPerformed();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Gm.player.Inventory.ConsumeSelectedItem())
            {
                Gm.OnPlayerCommandPerformed();
            }
        }
        else if (Gm.player.Inventory.carriedPowerups.Count >= 2)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Gm.player.Inventory.SelectNextPowerup();
                Gm.OnPlayerCommandPerformed();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Gm.player.Inventory.SelectPreviousPowerup();
                Gm.OnPlayerCommandPerformed();
            }
        }
    }

    public override void OnExit()
    {
        
    }
}