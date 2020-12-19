using System;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : GmAwareObject
{
    public HudOverlay Hud { get; private set; }
    public MainMenuOverlay MainMenuOverlay { get; private set; }
    public OnlineHighscoreOverlay OnlineHighscoreOverlay { get; private set; }
    public HighscoreEntryOverlay HighscoreEntryOverlay { get; private set; }

    private SettingsOverlay _settingsOverlayInGame;
    private SettingsOverlay _settingsOverlayFromMenu;

    public enum OverlayEnum 
    {
        MainMenu, Hud, GameOver, Registry, Highscore, Credits, 
        SettingsInGame, SettingsFromMenu, OnlineHighscore
    }
    private Overlay _activeOverlay;
    private Dictionary<OverlayEnum, Overlay> _overlays;

    public void SetActiveOverlay(OverlayEnum overlayEnumEnum)
    {
        _activeOverlay?.OnHide();
        _activeOverlay?.gameObject.SetActive(false);
    
        _activeOverlay = _overlays[overlayEnumEnum];
    
        _activeOverlay.gameObject.SetActive(true);
        _activeOverlay.OnShow();
    }

    public Overlay GetOverlay(OverlayEnum overlayEnum)
    {
        return _overlays[overlayEnum];
    }
    
    protected override void Awake()
    {
        base.Awake();

        _overlays = new Dictionary<OverlayEnum, Overlay>
        {
            {OverlayEnum.MainMenu, MainMenuOverlay = GetComponentInChildren<MainMenuOverlay>(true)},
            {OverlayEnum.Hud, Hud = GetComponentInChildren<HudOverlay>(true)},
            {OverlayEnum.SettingsInGame, _settingsOverlayInGame = GetComponentsInChildren<SettingsOverlay>(true)[0]},
            {OverlayEnum.SettingsFromMenu, _settingsOverlayFromMenu = GetComponentsInChildren<SettingsOverlay>(true)[1]},
            {OverlayEnum.GameOver, GetComponentInChildren<GameOverOverlay>(true)},
            {OverlayEnum.Registry, HighscoreEntryOverlay = GetComponentInChildren<HighscoreEntryOverlay>(true)},
            {OverlayEnum.Highscore, GetComponentInChildren<HighscoreOverlay>(true)},
            {OverlayEnum.Credits, GetComponentInChildren<CreditsOverlay>(true)},
            {OverlayEnum.OnlineHighscore, OnlineHighscoreOverlay = GetComponentInChildren<OnlineHighscoreOverlay>(true)}
        };
    }

    private void Start()
    {
        foreach (Overlay overlay in _overlays.Values)
        {
            overlay.gameObject.SetActive(false);
        }
        _overlays[OverlayEnum.MainMenu].gameObject.SetActive(true);
        SetActiveOverlay(OverlayEnum.MainMenu);
    }

    public void UpdateVolumeSliders(float musicVolume, float sfxVolume)
    {
        _settingsOverlayInGame.SetMusicVolumeSliderValue(musicVolume);
        _settingsOverlayFromMenu.SetMusicVolumeSliderValue(musicVolume);
        
        _settingsOverlayInGame.SetSfxVolumeSliderValue(sfxVolume);
        _settingsOverlayFromMenu.SetSfxVolumeSliderValue(sfxVolume);
    }
}