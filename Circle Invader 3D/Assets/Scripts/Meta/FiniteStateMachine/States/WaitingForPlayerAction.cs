using UnityEngine;

public class WaitingForPlayerAction : State
{
    public Player player;

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GM.currentPositionIndex--;
            if (GM.currentPositionIndex < 0)
            {
                GM.currentPositionIndex += 20;
            }

            player.SetTargetPos();
            GM.OnPlayerCommandPerformed();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GM.currentPositionIndex++;
            if (GM.currentPositionIndex > 20)
            {
                GM.currentPositionIndex -= 20;
            }

            player.SetTargetPos();
            GM.OnPlayerCommandPerformed();
        }
    }

    public override void OnExit()
    {
        
    }
}