using UnityEngine;

public class FieldItem : OrbitingObject
{
    public Powerup item;
    
    public Color colour;
    public Color upgradedColour;
    public int initDuration;

    [Header("Powerup variables")]
    [HideInInspector] public int remainingDuration;
    [HideInInspector] public bool isUpgraded;

    private float _offset;
    
    protected override void Start()
    {
        base.Start();
        Gm.PowerupManager.RegisterPowerup(this);
        // _scaleFactor = 1.0f / transform.localScale.x;
    }

    protected override void Update()
    {
        base.Update();

        // _offset += 0.01f;
        // Vector3 pos = transform.position;
        // pos = new Vector3(
        //     pos.x,
        //     pos.y + Mathf.Sin(_offset),
        //     pos.z);
        // transform.position = pos;

        transform.Rotate(new Vector3(0, 0.5f, 0));
        
        // TODO: Replace this with an Animator
    }

    public virtual void OnPickup()
    {
        if (!Gm.player.AddToInventory(item))
        {
            item.OnConsume();
        }
        Destroy(gameObject);
        Gm.PowerupManager.DeletePowerup(this);
    }
}