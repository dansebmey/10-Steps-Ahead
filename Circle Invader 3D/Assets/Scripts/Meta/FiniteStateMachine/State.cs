using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class State : MonoBehaviour
{
    protected GameManager Gm;
    protected FiniteStateMachine FSM;
    
    public void Init(GameManager gm, FiniteStateMachine fsm)
    {
        Gm = gm;
        Debug.Log("State's GM = " + Gm.GetInstanceID());
        FSM = fsm;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}