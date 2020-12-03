using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Powerup")]
public abstract class Powerup : ScriptableObject
{
    public Sprite inventoryIcon;
    protected GameManager Gm;

    protected virtual void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
        Debug.Log(Gm.name);
    }

    public abstract void OnConsume();
}