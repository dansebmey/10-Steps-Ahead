using System;
using UnityEngine;

public class Player : CIObject
{
    private float _distanceFromCenter = 4;

    protected override void Start()
    {
        base.Start();
        Gm.player = this;

        // _distanceFromCenter = transform.position.z;
        // 
        // foreach (Transform tf in GetComponentInChildren<Transform>())
        // {
        //     if (tf.CompareTag("PosMarker"))
        //     {
        //         targetPos = tf;
        //         targetPos.position = Vector3.left;
        //         Debug.Log("TargetPos = " + targetPos.position);
        //         break;
        //     }   
        // }
    }

    public void SetTargetPos()
    {
        targetPos = new Vector3(
            4 * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * Gm.currentPositionIndex),
            0,
            4 * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * Gm.currentPositionIndex));
        Debug.Log(GetInstanceID() + ": targetPos set to " + targetPos);
        
        Gm.SwitchState(typeof(PlayerMoving));
        // GM.SwitchState(typeof(InvokeEnemyAction));
    }
}