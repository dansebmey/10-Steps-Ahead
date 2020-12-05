using UnityEngine;

public class FieldItem : OrbitingObject
{
    public Item item;
    public bool consumeOnPickup;
    
    public Color colour;
    public Color upgradedColour;

    private int _remainingDuration;
    private int RemainingDuration
    {
        get => _remainingDuration;
        set
        {
            _remainingDuration = value;
            if (value <= 3)
            {
                // TODO: make it clear that the object is about to disappear
            }
            if (value <= 0)
            {
                Destroy();
            }
        }
    }

    private float _offset;
    
    protected override void Start()
    {
        base.Start();
        Gm.FieldItemManager.RegisterPowerup(this);
        RemainingDuration = Gm.FieldItemManager.itemLifespanInSteps;
    }

    protected override void Update()
    {
        base.Update();

        transform.Rotate(new Vector3(0, 0.5f, 0));
        // TODO: Replace this with an Animator
    }

    public void OnPickup()
    {
        if (!(item is Powerup powerup) || !Gm.player.AddToInventory(powerup))
        {
            item.OnConsume();
        }
        Destroy();
    }

    private void Destroy()
    {
        Gm.FieldItemManager.DeleteItem(this);
        Destroy(gameObject);
    }

    public void ReduceTimer()
    {
        RemainingDuration--;
    }
}