using System;
using UnityEngine;

public class Player : CIObject
{
    private float _distanceFromCenter = 4;

    protected override void Start()
    {
        base.Start();
        Gm.player = this;
        Debug.Log("Player's id @ Start(): "+GetInstanceID());

        // _distanceFromCenter = transform.position.z;
    }

    public void SetTargetPos()
    {
        Debug.Log("Player's id @ SetTargetPos(): "+GetInstanceID());
        var gm = FindObjectOfType<GameManager>();
        targetPos = new Vector3(
            4 * Mathf.Cos((Mathf.PI * 2 / gm.BarrierManager.amountOfBarriers) * gm.CurrentPosIndex),
            0,
            4 * Mathf.Sin((Mathf.PI * 2 / gm.BarrierManager.amountOfBarriers) * gm.CurrentPosIndex));
        
        // gm.SwitchState(typeof(PlayerMoving));
        gm.SwitchState(typeof(InvokeEnemyAction));
    }

    protected override void AngleTowardsSomething()
    {
        transform.LookAt(Vector3.zero);
    }
}