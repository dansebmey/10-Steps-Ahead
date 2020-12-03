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

    protected abstract void OnPickup();
}