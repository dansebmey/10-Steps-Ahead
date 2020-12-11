using UnityEngine;

public class HighscoreState : State
{
    public override void OnEnter()
    {
        Debug.LogError("SHOULD NOT ENTER THIS STATE - REMOVAL PENDING");
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Gm.SwitchState(typeof(MenuState));
        }
    }

    public override void OnExit()
    {
        
    }
}