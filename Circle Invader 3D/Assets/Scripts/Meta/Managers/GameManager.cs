using System;
using System.Collections;
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
    public OnlineHighscoreManager OnlineHighscoreManager { get; private set; }
    public TutorialManager TutorialManager { get; private set; }
    
    public AudioManager AudioManager => AudioManager.GetInstance();
    public LowPassFilterManager LowPassFilterManager { get; private set; }
    public AestheticsManager AestheticsManager { get; private set; }

    [SerializeField] private State[] statePrefabs;

    public Player player;
    public MechaTotem enemy;

    public void StartNewGame()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Hud);

        if (PlayerScore > 0)
        {
            PlayerScore = 0;
            foreach (IResetOnGameStart resetter in _onGameStartResetters)
            {
                resetter.OnNewGameStart();
            }
        }
        
        TutorialManager.ShowInstruction(TutorialManager.InstructionEnum.Movement, KeyCode.A, KeyCode.D);
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
        OnlineHighscoreManager = GetComponent<OnlineHighscoreManager>();
        AestheticsManager = GetComponent<AestheticsManager>();
        TutorialManager = GetComponent<TutorialManager>();
        
        CameraController = FindObjectOfType<CameraController>();
        OverlayManager = CameraController.GetComponentInChildren<OverlayManager>();
        LowPassFilterManager = CameraController.GetComponent<LowPassFilterManager>();
        
        _fsm = new FiniteStateMachine(this, typeof(MenuState), statePrefabs);
        
        _playerCommandListeners = new ConcurrentStack<IPlayerCommandListener>();
        _playerCommandListeners.Push(FieldItemManager);
        _playerCommandListeners.Push(enemy);
        _playerCommandListeners.Push(TutorialManager);

        _onGameStartResetters = new List<IResetOnGameStart> {player, enemy, BarrierManager, FieldItemManager};
    }

    private void Start()
    {
        LoadSavedData();
        AudioManager.PlayMusic();
    }

    private void LoadSavedData()
    {
        GameData gameData = _dataManager.LoadSavedGame();
        if (gameData != null)
        {
            PlayerScore = gameData.playerScore;
            CurrentPosIndex = gameData.playerData.posIndex;
            
            foreach (IResetOnGameStart resetter in _onGameStartResetters)
            {
                resetter.OnGameLoad(gameData);
            }

            OverlayManager.MainMenuOverlay.EnableContinueButton();
        }

        PersistentData persistentData = _dataManager.LoadSettings();
        if (persistentData != null)
        {
            AudioManager.MusicVolume = persistentData.musicVolume;
            AudioManager.SfxVolume = persistentData.sfxVolume;
            OverlayManager.UpdateVolumeSliders(persistentData.musicVolume, persistentData.sfxVolume);
            
            AestheticsManager.IsDyslexicFontShown = persistentData.isDyslexicFontShown;
            
            ((RegistryOverlay) OverlayManager.GetOverlay(OverlayManager.OverlayEnum.Registry)).SetHighscoreName(
                persistentData.lastEnteredHighscoreName);
        }
    }

    private void Update()
    {
        CurrentState.OnUpdate();
    }

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        AudioManager.Play("Slide");
        
        PlayerScore++;
        FieldItemManager.HandlePowerupCheck();

        foreach (IPlayerCommandListener pcl in _playerCommandListeners)
        {
            pcl.OnPlayerCommandPerformed(keyCode);
        }

        SwitchState(typeof(InvokeEnemyAction));
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
        player.isDefeated = true;
        AudioManager.Play("GameOver");
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

    public void ShowCreditsOverlay()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Credits);
    }

    public void ShowSettingsInGame()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.SettingsInGame);
    }

    public void ShowSettingsFromMenu()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.SettingsFromMenu);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Hud);
    }

    public void ShowControlsOverlay()
    {
        
    }

    public void ShowGlobalHighscoreOverlay()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.OnlineHighscore);
    }

    private void OnApplicationQuit()
    {
        _dataManager.Save(this);
    }
}