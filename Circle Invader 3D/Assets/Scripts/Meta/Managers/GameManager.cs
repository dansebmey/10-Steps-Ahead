using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }
    public FieldItemManager FieldItemManager { get; private set; }

    private ConcurrentStack<IPlayerCommandListener> _playerCommandListeners;
    private List<IResetOnGameStart> _onGameStartResetters;
    
    private FiniteStateMachine _fsm;
    private DataManager _dataManager;
    public CameraController CameraController { get; private set; }
    public OverlayManager OverlayManager { get; private set; }
    public HighscoreManager HighscoreManager { get; private set; }
    private PermaOverlay _permaOverlay;
    
    public AudioManager AudioManager => AudioManager.GetInstance();
    private LowPassFilterManager _lowPassFilterManager;

    [SerializeField] private State[] statePrefabs;

    public Player player;
    public MechaTotem enemy;

    [HideInInspector] public List<IDamageable> damageables;

    public void StartGame()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Hud);
        SwitchState(typeof(WaitingForPlayerAction));
        
        CameraController.FocusOn(player.transform, new Vector3(0, 5, -5), new Vector3(25, 0, 0));

        if (PlayerScore > 0)
        {
            PlayerScore = 0;
            foreach (IResetOnGameStart resetter in _onGameStartResetters)
            {
                resetter.OnGameReset();
            }
        }
        
        OverlayManager.PermaOverlay.ShowInstruction(
            PermaOverlay.InstructionEnum.Movement,
            KeyCode.A, KeyCode.D);
    }

    public void ShowHighscores()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Highscore);
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
        _dataManager = GetComponent<DataManager>();
        HighscoreManager = GetComponent<HighscoreManager>();
        
        CameraController = FindObjectOfType<CameraController>();
        OverlayManager = CameraController.GetComponentInChildren<OverlayManager>();
        _lowPassFilterManager = CameraController.GetComponent<LowPassFilterManager>();
        _permaOverlay = CameraController.GetComponentInChildren<PermaOverlay>();
        
        _fsm = new FiniteStateMachine(this, typeof(MenuState), statePrefabs);
    }

    private void Start()
    {
        damageables = new List<IDamageable>();

        _playerCommandListeners = new ConcurrentStack<IPlayerCommandListener>();
        _playerCommandListeners.Push(FieldItemManager);
        _playerCommandListeners.Push(_lowPassFilterManager);
        _playerCommandListeners.Push(enemy);
        _playerCommandListeners.Push(_permaOverlay);
        
        _onGameStartResetters = new List<IResetOnGameStart>();
        _onGameStartResetters.Add(player);
        _onGameStartResetters.Add(enemy);
        _onGameStartResetters.Add(BarrierManager);
        _onGameStartResetters.Add(FieldItemManager);

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

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        // if(!(CurrentState is GameOverState))
        // {
            AudioManager.Play("Slide", 0.1f);
            
            PlayerScore++;
            FieldItemManager.HandlePowerupCheck();

            foreach (IPlayerCommandListener pcl in _playerCommandListeners)
            {
                pcl.OnPlayerCommandPerformed(keyCode);
            }

            SwitchState(typeof(InvokeEnemyAction));
        // }
    }

    public void ApplyDamage(int damageDealt, int rawPosIndex = -99)
    {
        int posIndex = rawPosIndex == -99 ? CurrentPosIndex : WrapPosIndex(rawPosIndex);

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
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.GameOver);
    }

    public void RestartApplication()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenuPressed()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.MainMenu);
    }

    public void RegisterScorePressed()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Registry);
    }

    public void ShowCreditOverlay()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Credits);
    }
}