using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }
    public FieldItemManager FieldItemManager { get; private set; }

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
            value = WrapPosIndex(value);

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
        FieldItemManager = GetComponentInChildren<FieldItemManager>();
        _hud = FindObjectOfType<HeadsUpDisplay>();
    }

    private void Start()
    {
        _fsm = new FiniteStateMachine(this, typeof(WaitingForPlayerAction), statePrefabs);
        damageables = new List<IDamageable>();

        _playerCommandListeners = new List<IPlayerCommandListener>
        {
            BarrierManager,
            FieldItemManager
        };

        AudioManager.FadeVolume("Soundtrack", 0, AudioManager.FindSound("Soundtrack").initVolume, 2);
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
            FieldItemManager.HandlePowerupCheck();
            
            foreach (IPlayerCommandListener pcl in _playerCommandListeners)
            {
                pcl.OnPlayerCommandPerformed();
            }
            
            SwitchState(typeof(InvokeEnemyAction));
        }
    }

    public void ApplyDamage(int damageDealt, int rawPosIndex = -1)
    {
        int posIndex = rawPosIndex == -1 ? CurrentPosIndex : WrapPosIndex(rawPosIndex);

        if (BarrierManager.IsBarrierDormant(posIndex) && player.CurrentPosIndex == posIndex)
        {
            SwitchState(typeof(GameOverState));
        }
        else
        {
            BarrierManager.DamageBarrier(damageDealt, posIndex);
            SwitchState(typeof(WaitingForPlayerAction));
        }
    }

    public bool IsScoreHigherThan(int value)
    {
        return value >= PlayerScore;
    }

    public int WrapPosIndex(int posIndex)
    {
        if (posIndex < 0)
        {
            posIndex += BarrierManager.amountOfBarriers;
        }
        else if (posIndex >= BarrierManager.amountOfBarriers)
        {
            posIndex -= BarrierManager.amountOfBarriers;
        }

        return posIndex;
    }
}