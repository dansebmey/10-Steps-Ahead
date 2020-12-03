using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }
    public PowerupManager PowerupManager { get; private set; }

    private List<IPlayerCommandListener> _playerCommandListeners;
    private FiniteStateMachine _fsm;
    private HeadsUpDisplay _hud;
    public AudioManager AudioManager => AudioManager.GetInstance();

    [SerializeField] private State[] statePrefabs;

    public Player player;
    public MechaTotem enemy;

    [HideInInspector] public List<IDamageable> damageables;
    
    public void SwitchState(Type newStateType)
    {
        _fsm.SwitchState(newStateType);
    }

    public State CurrentState => _fsm.CurrentState;

    private int _currentPosIndex;
    public int CurrentPosIndex
    {
        get => _currentPosIndex;
        set
        {
            if (value < 0)
            {
                value += BarrierManager.amountOfBarriers;
            }
            else if (value >= BarrierManager.amountOfBarriers)
            {
                value -= BarrierManager.amountOfBarriers;
            }

            _currentPosIndex = value;
            player.CurrentPosIndex = CurrentPosIndex;
        }
    }

    private int _playerScore;
    public int PlayerScore
    {
        get => _playerScore;
        private set
        {
            _playerScore = value;
            _hud.UpdateScore(value);
        }
    }

    private void Awake()
    {
        BarrierManager = GetComponentInChildren<BarrierManager>();
        PowerupManager = GetComponentInChildren<PowerupManager>();
        _hud = FindObjectOfType<HeadsUpDisplay>();
    }

    private void Start()
    {
        _fsm = new FiniteStateMachine(this, typeof(WaitingForPlayerAction), statePrefabs);
        damageables = new List<IDamageable>();

        _playerCommandListeners = new List<IPlayerCommandListener>
        {
            BarrierManager,
            PowerupManager
        };

        AudioManager.FadeVolume("Soundtrack", 0, 0.5f, 2);
        AudioManager.Play("Soundtrack");
    }

    private void Update()
    {
        CurrentState.OnUpdate();
    }

    public void RegisterObject(MovableObject obj)
    {
        if (obj is IDamageable dam)
        {
            damageables.Add(dam);
        }
    }

    public void OnPlayerCommandPerformed()
    {
        if(!(CurrentState is GameOverState))
        {
            AudioManager.Play("Slide", 0.1f);
            PlayerScore++;
            PowerupManager.HandlePowerupCheck();
            
            foreach (IPlayerCommandListener pcl in _playerCommandListeners)
            {
                pcl.OnPlayerCommandPerformed();
            }
            
            SwitchState(typeof(InvokeEnemyAction));
        }
    }

    public void ApplyDamage(int damageDealt, int currentPosIndex = -1)
    {
        if (currentPosIndex == -1)
        {
            currentPosIndex = _currentPosIndex;
        }

        if (BarrierManager.IsBarrierDormant(currentPosIndex))
        {
            SwitchState(typeof(GameOverState));
        }
        else
        {
            BarrierManager.DamageBarrier(damageDealt, currentPosIndex);
            SwitchState(typeof(WaitingForPlayerAction));
        }
    }

    public bool IsScoreHigherThan(int value)
    {
        return value >= PlayerScore;
    }
}