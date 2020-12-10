using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }
    public FieldItemManager FieldItemManager { get; private set; }

    private ConcurrentStack<IPlayerCommandListener> _playerCommandListeners;
    
    private FiniteStateMachine _fsm;
    private DataManager _dataManager;
    private CameraController _cameraController;
    public OverlayManager OverlayManager { get; private set; }
    public HighscoreManager HighscoreManager { get; private set; }
    
    public AudioManager AudioManager => AudioManager.GetInstance();
    private LowPassFilterManager _lowPassFilterManager;

    [SerializeField] private State[] statePrefabs;

    public Player player;
    public MechaTotem enemy;

    [HideInInspector] public List<IDamageable> damageables;

    public void StartGame()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.HUD);
        SwitchState(typeof(WaitingForPlayerAction));
        
        // targetPos = new Vector3(0, 11.33f, -5);
        _cameraController.FocusOn(player.transform, new Vector3(0, 5, -5), new Vector3(25, 0, 0), 2);
    }

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
            OverlayManager.Hud.UpdateScore(value);
        }
    }

    private void Awake()
    {
        BarrierManager = GetComponentInChildren<BarrierManager>();
        FieldItemManager = GetComponentInChildren<FieldItemManager>();
        OverlayManager = FindObjectOfType<OverlayManager>();
        
        _lowPassFilterManager = FindObjectOfType<CameraController>().GetComponent<LowPassFilterManager>();
        _dataManager = GetComponent<DataManager>();
        _cameraController = FindObjectOfType<CameraController>();
        HighscoreManager = GetComponent<HighscoreManager>();
    }

    private void Start()
    {
        _fsm = new FiniteStateMachine(this, typeof(MainMenuState), statePrefabs);
        damageables = new List<IDamageable>();

        _playerCommandListeners = new ConcurrentStack<IPlayerCommandListener>();
        _playerCommandListeners.Push(BarrierManager);
        _playerCommandListeners.Push(FieldItemManager);
        _playerCommandListeners.Push(_lowPassFilterManager);
        _playerCommandListeners.Push(enemy);

        AudioManager.FadeVolume("Soundtrack", 0, AudioManager.FindSound("Soundtrack").initVolume, 2);
        AudioManager.Play("Soundtrack");
    }

    public void RegisterPlayerCommandListener(IPlayerCommandListener listener)
    {
        _playerCommandListeners.Push(listener);
    }

    public void DeletePlayerCommandListener(IPlayerCommandListener listener)
    {
        _playerCommandListeners.TryPop(out listener);
        Debug.Log("Removed listener ["+listener+"]");
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

        if (BarrierManager.IsBarrierCollapsed(posIndex) && player.CurrentPosIndex == posIndex)
        {
            EndGame();
        }
        else
        {
            BarrierManager.DamageBarrier(damageDealt, posIndex);
            SwitchState(typeof(WaitingForPlayerAction));
        }
    }

    public bool IsScoreHigherThan(int req)
    {
        return PlayerScore >= req;
    }

    private int WrapPosIndex(int posIndex)
    {
        return (BarrierManager.amountOfBarriers + posIndex) % BarrierManager.amountOfBarriers;
    }

    private void EndGame()
    {
        SwitchState(typeof(GameOverState));
    }
}