using UnityEngine;

public class InvokeEnemyAction : State
{
    public override void OnEnter()
    {
        GM.enemy.InvokeNextAction();
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}