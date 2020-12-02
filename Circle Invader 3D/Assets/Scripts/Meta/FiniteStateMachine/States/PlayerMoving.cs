using UnityEngine;

public class PlayerMoving : State
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        Transform pTf = GM.player.transform;
        Vector3 tPos = GM.player.targetPos;
        
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