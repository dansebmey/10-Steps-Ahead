using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

public class FieldItem : OrbitingObject
{
    public Item item;
    public int scoreReq;
    [Range(0,100)] public int spawnWeight;
    public bool consumeOnPickup;

    private Text3D durationText;

    private int _remainingDuration;

    public int RemainingDuration
    {
        get => _remainingDuration;
        set
        {
            _remainingDuration = value;
            durationText.Text = value.ToString();
            switch (value)
            {
                case 6:
                    durationText.EnableWarningColour1();
                    break;
                case 3:
                    durationText.EnableWarningColour2();
                    break;
                case 0:
                    Destroy();
                    break;
            }
        }
    }

    private float _offset;

    protected override void Awake()
    {
        base.Awake();
        durationText = GetComponentInChildren<Text3D>();
    }

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

    public virtual void Destroy()
    {
        Gm.FieldItemManager.DeleteItem(this);
        Destroy(gameObject);
    }

    public void ReduceTimer()
    {
        RemainingDuration--;
    }
}