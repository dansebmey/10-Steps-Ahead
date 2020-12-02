using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class State : MonoBehaviour
{
    protected GameManager GM => GameManager.Instance;
    protected FiniteStateMachine FSM;
    
    public void Init(FiniteStateMachine fsm)
    {
        Debug.Log("State's GM = " + GM.GetInstanceID());
        FSM = fsm;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}