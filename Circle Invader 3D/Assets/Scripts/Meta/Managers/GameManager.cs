using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }
    public int currentPositionIndex;

    private FiniteStateMachine _fsm;
    
    [SerializeField] private State[] statePrefabs;

    public Player player;
    public MechaTotem enemy;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else
            {
                Debug.Log("GameManager already has an instance!");
            }
        }
    }
    
    public void SwitchState(Type newStateType)
    {
        _fsm.SwitchState(newStateType);
    }

    private State CurrentState => _fsm.currentState;

    private void Awake()
    {
        Instance = this;
        BarrierManager = GetComponentInChildren<BarrierManager>();
    }

    private void Start()
    {
        _fsm = new FiniteStateMachine(typeof(WaitingForPlayerAction), statePrefabs);
    }

    private void Update()
    {
        CurrentState.OnUpdate();
    }

    public void OnPlayerCommandPerformed()
    {
        
    }
}