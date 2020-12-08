using UnityEngine;

public class DelayedProjectile : OrbitingObject
{
    private int _stepsTaken;

    protected override void Start()
    {
        base.Start();
        transform.position = DetermineTargetPos();
    }

    public bool MoveForward()
    {
        _stepsTaken++;
        distanceFromCenter = 0.5f + _stepsTaken * 0.8f;
        if (_stepsTaken == 4)
        {
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