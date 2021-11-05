using UnityEngine;
using UnityEngine.UI;

public class Missile : OrbitingObject
{
    private Text3D durationText;
    
    public int StepsTaken { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        durationText = GetComponentInChildren<Text3D>();
    }

    protected override void Start()
    {
        base.Start();
        
        StepsTaken = -1;
        MoveForward();
    }

    public bool MoveForward()
    {
        StepsTaken++;
        SetStepsTaken(StepsTaken);
        if (StepsTaken == 4)
        {
            Gm.AudioManager.Play("BasicAttack");
            Gm.ApplyDamage(1, CurrentPosIndex);
            Destroy(gameObject);
            return false;
        }
        
        return true;
    }

    private Vector3 DetermineTargetPos()
    {
        return new Vector3(
            distanceFromCenter * Mathf.Cos((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex),
            transform.position.y,
            distanceFromCenter * Mathf.Sin((Mathf.PI * 2 / Gm.BarrierManager.amountOfBarriers) * CurrentPosIndex));
    }

    public void SetStepsTaken(int stepsTaken)
    {
        StepsTaken = stepsTaken;
        
        durationText.Text = (4 - stepsTaken).ToString();
        if (stepsTaken == 2)
        {
            durationText.EnableWarningColour1();
        }
        else if (stepsTaken == 3)
        {
            durationText.EnableWarningColour2();
        }
        
        distanceFromCenter = 1.25f + stepsTaken * (Gm.player.distanceFromCenter * 0.125f);
        targetPos = DetermineTargetPos();
    }
}