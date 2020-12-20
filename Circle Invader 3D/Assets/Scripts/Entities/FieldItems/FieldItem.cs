using UnityEngine;
using UnityEngine.Events;

public class FieldItem : OrbitingObject
{
    public Item item;
    public int scoreReq;
    [Range(0,100)] public int spawnWeight;
    public bool consumeOnPickup;

    private int _remainingDuration;

    public int RemainingDuration
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
        
        if (RemainingDuration == 0) // returns false if the item was loaded from save data
        {
            RemainingDuration = Gm.FieldItemManager.itemLifespanInSteps;
        }
    }

    protected override void Update()
    {
        base.Update();

        transform.Rotate(new Vector3(0, 0.5f, 0));
        // TODO: Replace this with an Animator
    }

    public virtual void OnPickup()
    {
        if (!(item is Powerup powerup) || !Gm.player.AddToInventory(powerup))
        {
            item.OnConsume();
        }
        Destroy();
    }

    protected virtual void Destroy()
    {
        Gm.FieldItemManager.DeleteItem(this);
        Destroy(gameObject);
    }

    public void ReduceTimer()
    {
        RemainingDuration--;
    }
}