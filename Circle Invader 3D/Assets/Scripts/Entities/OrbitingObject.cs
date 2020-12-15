using UnityEngine;

public abstract class OrbitingObject : MovableObject
{
    private int _currentPosIndex;
    [HideInInspector] public float distanceFromCenter;

    protected override void Start()
    {
        base.Start();
        distanceFromCenter = Gm.BarrierManager.barrierDistanceFromCenter + 0.75f;
    }

    public int CurrentPosIndex
    {
        get => _currentPosIndex;
        set
        {
            if (value < 0)
            {
                value += Gm.BarrierManager.amountOfBarriers;
            }
            else if (value >= Gm.BarrierManager.amountOfBarriers)
            {
                value -= Gm.BarrierManager.amountOfBarriers;
            }
            
            _currentPosIndex = value;
            targetPos = new Vector3(
                distanceFromCenter * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * _currentPosIndex),
                0,
                distanceFromCenter * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * _currentPosIndex));
        }
    }
}