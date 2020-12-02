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
            Gm.player.SetTargetPos();
            Gm.OnPlayerCommandPerformed();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Gm.CurrentPosIndex++;
            Gm.player.SetTargetPos();
            Gm.OnPlayerCommandPerformed();
        }
    }

    public override void OnExit()
    {
        
    }
}