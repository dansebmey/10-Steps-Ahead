using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class State
{
    protected GameManager Gm;
    
    public virtual void Init(GameManager gm)
    {
        Gm = gm;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}

