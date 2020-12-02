using UnityEngine;

public class PlayerMoving : State
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        Transform pTf = Gm.player.transform;
        Vector3 tPos = Gm.player.targetPos;
        
        if (pTf.position != tPos)
        {
            pTf.position = Vector3.Lerp(
                pTf.position,
                tPos,
                0.25f);
            pTf.LookAt(Vector3.zero);
        }
        else
        {
            FSM.SwitchState(typeof(InvokeEnemyAction));
        }
    }

    public override void OnExit()
    {
        
    }
}