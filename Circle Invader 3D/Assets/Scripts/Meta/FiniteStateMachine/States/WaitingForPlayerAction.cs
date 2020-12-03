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
    }

    public override void OnExit()
    {
        
    }
}