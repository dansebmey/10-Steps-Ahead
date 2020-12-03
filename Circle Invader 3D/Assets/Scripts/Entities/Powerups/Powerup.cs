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

    protected override void Start()
    {
        base.Start();
        Gm.PowerupManager.RegisterPowerup(this);
    }

    public virtual void OnPickup()
    {
        Debug.Log(name + " was picked up!");
    }
}