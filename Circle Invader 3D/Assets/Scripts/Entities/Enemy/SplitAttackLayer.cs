using UnityEngine;

public class SplitAttackLayer : TotemLayer
{
    // protected override void Start()
    // {
    //     base.Start();
    //
    //     Transform[] cTf = GetComponentsInChildren<Transform>();
    //     
    //     // Left barrel
    //     cTf[2].localPosition = new Vector3(
    //         0.675f * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * (Gm.CurrentPosIndex-1)),
    //         0.5f, 
    //         0.675f * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * (Gm.CurrentPosIndex-1)));
    //     cTf[2].rotation = Quaternion.LookRotation(transform.position - Vector3.zero);
    //     
    //     // Right barrel
    //     cTf[3].localPosition = new Vector3(
    //         0.675f * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * (Gm.CurrentPosIndex+1)),
    //         0.5f,
    //         0.675f * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * (Gm.CurrentPosIndex+1)));
    //     cTf[3].rotation = Quaternion.LookRotation(transform.position - Vector3.zero);
    // }
}