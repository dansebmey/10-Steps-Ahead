using UnityEngine;

public class Missile : OrbitingObject
{
    public int StepsTaken { get; private set; }

    protected override void Start()
    {
        base.Start();
        transform.position = DetermineTargetPos();
    }

    public bool MoveForward()
    {
        StepsTaken++;
        distanceFromCenter = 0.5f + StepsTaken * 0.8f;
        if (StepsTaken == 4)
        {
            Gm.AudioManager.Play("BasicAttack", 0.05f);
            Gm.ApplyDamage(1, CurrentPosIndex);
            Destroy(gameObject);
            return false;
        }
        
        targetPos = DetermineTargetPos();
        return true;
    }

    private Vector3 DetermineTargetPos()
    {
        return new Vector3(
            distanceFromCenter * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex),
            0.5f,
            distanceFromCenter * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex));
    }
}