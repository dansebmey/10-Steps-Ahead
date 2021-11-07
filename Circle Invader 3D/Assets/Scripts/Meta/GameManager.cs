using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public bool IOEnabled = true;
    
    public Player player;
    public Enemy enemy;
    
    public BarrierManager BarrierManager { get; private set; }
    public FieldItemManager FieldItemManager { get; private set; }
    public AchievementManager AchievementManager { get; private set; }

    private ConcurrentStack<IPlayerCommandListener> _playerCommandListeners;
    private List<IResetOnGameStart> _onGameStartResetters;
    
    [SerializeField] private Text mitigatedDamageCounterText;
    
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
        OverlayManager.SetActiveOverlay(IOEnabled
            ? OverlayManager.OverlayEnum.Highscore
            : OverlayManager.OverlayEnum.OnlineHighscore);
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
    
    private int _damageTaken;
    public int DamageTaken
    {
        get => _damageTaken;
        set
        {
            _damageTaken = value;
            UpdateDamageMitigatedToLostRatio();
        }
    }

    private int _damageMitigated;
    public int DamageMitigated
    {
        get => _damageMitigated;
        set
        {
            _damageMitigated = value;
            UpdateDamageMitigatedToLostRatio();
        }
    }


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
        AchievementManager = GetComponentInChildren<AchievementManager>();
        
        _dataManager = GetComponent<DataManager>();
        HighscoreManager = GetComponent<HighscoreManager>();
        OnlineHighscoreManager = GetComponent<OnlineHighscoreManager>();
        AestheticsManager = GetComponent<AestheticsManager>();
        TutorialManager = GetComponent<TutorialManager>();
        
        CameraController = FindObjectOfType<CameraController>();
        OverlayManager = CameraController.GetComponentInChildren<OverlayManager>();
        LowPassFilterManager = CameraController.GetComponent<LowPassFilterManager>();
        
        _fsm = new FiniteStateMachine(this, typeof(MenuState));
    }

    private void Start()
    {
        _playerCommandListeners = new ConcurrentStack<IPlayerCommandListener>();
        _playerCommandListeners.Push(FieldItemManager);
        _playerCommandListeners.Push(enemy);
        _playerCommandListeners.Push(TutorialManager);
        _playerCommandListeners.Push(OverlayManager.Hud.AchievementProgressPanel);

        _onGameStartResetters = new List<IResetOnGameStart>
        {
            player,
            enemy, 
            BarrierManager,
            FieldItemManager,
            AchievementManager,
            OverlayManager.Hud.AchievementProgressPanel
        };
        
        if (IOEnabled) LoadSavedData();
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
            
            ((HighscoreEntryOverlay) OverlayManager.GetOverlay(OverlayManager.OverlayEnum.Registry)).SetHighscoreName(
                persistentData.lastEnteredHighscoreName);
        }
    }

    private void UpdateDamageMitigatedToLostRatio()
    {
        string ratioString = ((DamageMitigated / DamageTaken) * 100).ToString();
        mitigatedDamageCounterText.text = ratioString.Substring(0, Math.Min(ratioString.Length, 3));
    }

    [HideInInspector] public float timePassed;
    private void Update()
    {
        CurrentState.OnUpdate();
        
        if (!(CurrentState is MenuState))
        {
            timePassed += Time.deltaTime;
        }
    }

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        if (_playerScore == 1)
        {
            StartCoroutine(StartClock());
        }
        
        AudioManager.Play("Slide");
        FieldItemManager.HandlePowerupCheck();
        AchievementManager.IncrementStepCounter();
        BarrierManager.HandleEqualBarrierHPCheck();
        
        if (!player.isDefeated)
        {
            SwitchState(typeof(InvokeEnemyActionState));
        }
        
        foreach (IPlayerCommandListener pcl in _playerCommandListeners)
        {
            pcl.OnPlayerCommandPerformed(keyCode);
        }

        PlayerScore++;
    }

    private IEnumerator StartClock()
    {
        bool speedrunMarkPassed = false;
        
        while (!player.isDefeated)
        {
            OverlayManager.Hud.UpdateClock((int)timePassed);

            if (!speedrunMarkPassed && timePassed >= 60)
            {
                speedrunMarkPassed = true;
                
                EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.ResetAchievementProgress,
                    AchievementManager.AchievementType.Speedrun, _playerScore);
                // EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
                //     AchievementManager.AchievementType.Speedrun, _playerScore);

                if (_playerScore >= 120)
                {
                    AchievementManager.QueueAchievementToShow(new Achievement
                    {
                        achievementName = "Speedrun Result",
                        achievementDescription = "You reached a score of " + _playerScore + " within the first minute!"
                    });
                }
            }
            
            yield return new WaitForSeconds(1);
        }
    }

    public void ToggleClock()
    {
        OverlayManager.Hud.ToggleClock();
    }

    public int ApplyDamage(int damageDealt, int rawPosIndex = -99)
    {
        int posIndex = rawPosIndex == -99 ? CurrentPosIndex : WrapPosIndex(rawPosIndex);

        if (BarrierManager.IsBarrierCollapsed(posIndex) && player.CurrentPosIndex == posIndex)
        {
            EndGame();
            return damageDealt;
        }
        
        int hpLost = BarrierManager.DamageBarrier(damageDealt, posIndex);
        DamageTaken += hpLost;
        if (hpLost < damageDealt)
        {
            DamageMitigated += Math.Abs(damageDealt - hpLost);
            UpdateDamageMitigatedToLostRatio();
        }
        
        SwitchState(typeof(WaitingForPlayerActionState));
        return hpLost;
    }

    public bool IsScoreHigherThan(int req)
    {
        return PlayerScore >= req;
    }

    public int WrapPosIndex(int posIndex)
    {
        return (BarrierManager.amountOfBarriers + posIndex) % BarrierManager.amountOfBarriers;
    }

    public void EndGame()
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

    public void ShowAchievementsInGame()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.AchievementsInGame);
    }
    
    public void ShowAchievementsFromMenu()
    {
        OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.AchievementsFromMenu);
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