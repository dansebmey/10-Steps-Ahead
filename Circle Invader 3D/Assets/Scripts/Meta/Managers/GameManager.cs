using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }

    private FiniteStateMachine _fsm;
    
    [SerializeField] private State[] statePrefabs;

    public Player player;
    public MechaTotem enemy;

    [HideInInspector] public List<IDamageable> damageables;
    
    public void SwitchState(Type newStateType)
    {
        _fsm.SwitchState(newStateType);
    }

    private State CurrentState => _fsm.currentState;

    private int _currentPosIndex;
    public int CurrentPosIndex
    {
        get => _currentPosIndex;
        set
        {
            if (value < 0)
            {
                value += 20;
            }
            else if (value >= 20)
            {
                value -= 20;
            }

            _currentPosIndex = value;
        }
    }

    private void Awake()
    {
        BarrierManager = GetComponentInChildren<BarrierManager>();
    }

    private void Start()
    {
        _fsm = new FiniteStateMachine(this, typeof(WaitingForPlayerAction), statePrefabs);
        damageables = new List<IDamageable>();
    }

    private void Update()
    {
        CurrentState.OnUpdate();
    }

    public void RegisterObject(CIObject obj)
    {
        if (obj is IDamageable dam)
        {
            damageables.Add(dam);
        }
    }

    public void OnPlayerCommandPerformed()
    {
        
    }

    public void ApplyDamage(int damageDealt, int currentPosIndex = -1)
    {
        if (currentPosIndex == -1)
        {
            currentPosIndex = _currentPosIndex;
        }
        
        if (BarrierManager.IsBarrierDormant(currentPosIndex))
            SwitchState(typeof(GameOverState));
        else
            BarrierManager.DamageBarrier(damageDealt, currentPosIndex);
    }
}