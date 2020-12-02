using UnityEngine;

public class WaitingForPlayerAction : State
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Gm.CurrentPosIndex--;
            Gm.player.SetTargetPos();
            Gm.OnPlayerCommandPerformed();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
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