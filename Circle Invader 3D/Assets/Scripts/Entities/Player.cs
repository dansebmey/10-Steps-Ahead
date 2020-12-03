using System;
using UnityEngine;

public class Player : OrbitingObject
{
    private float _distanceFromCenter = 4;

    protected override void Start()
    {
        base.Start();
        Gm.player = this;
        
        targetPos = new Vector3(
            _distanceFromCenter * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * Gm.CurrentPosIndex),
            0,
            _distanceFromCenter * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * Gm.CurrentPosIndex));
        transform.position = targetPos;
        transform.LookAt(Vector3.zero);
    }

    public void SetTargetPos()
    {
        targetPos = new Vector3(
            _distanceFromCenter * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * Gm.CurrentPosIndex),
            0,
            _distanceFromCenter * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * Gm.CurrentPosIndex));
        
        Gm.SwitchState(typeof(InvokeEnemyAction));
    }

    protected override void AngleTowardsSomething()
    {
        transform.LookAt(Vector3.zero);
    }
}