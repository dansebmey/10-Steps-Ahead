using UnityEngine;

public abstract class Powerup : OrbitingObject
{
    public MeshRenderer model;
    public Color colour;
    public Color upgradedColour;

    [Header("Powerup variables")]
    public int initDuration;
    [HideInInspector] public int remainingDuration;
    [HideInInspector] public bool isUpgraded;
    
    
    private float _offset;
    private float _scaleFactor;

    
    protected override void Start()
    {
        base.Start();
        Gm.PowerupManager.RegisterPowerup(this);
        _scaleFactor = 1.0f / transform.localScale.x;
    }

    protected override void Update()
    {
        base.Update();

        _offset += 0.01f;
        Vector3 pos = transform.position;
        pos = new Vector3(
            pos.x,
            pos.y + _scaleFactor * 0.33f * Mathf.Sin(_offset),
            pos.z);
        transform.position = pos;

        transform.Rotate(new Vector3(0, 0.5f, 0));
        
        // TODO: Replace this with an Animator
    }

    public virtual void OnPickup()
    {
        if (Gm.player.AddToInventory(this))
        {
            Debug.Log(name + " was picked up!");
            transform.parent = Gm.player.transform;
            // Destroy(gameObject);
        }
    }
}