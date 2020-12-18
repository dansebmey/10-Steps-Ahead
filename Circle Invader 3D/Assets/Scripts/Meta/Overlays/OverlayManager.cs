using System;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : GmAwareObject
{
    public HudOverlay Hud { get; private set; }
    public SettingsOverlay SettingsOverlay { get; private set; }
    public MainMenuOverlay MainMenuOverlay { get; private set; }
    public OnlineHighscoreOverlay OnlineHighscoreOverlay { get; private set; }
    public RegistryOverlay RegistryOverlay { get; private set; }
    
    private Dictionary<OverlayEnum, Overlay> _overlays;
    private Overlay _activeOverlay;
    public enum OverlayEnum { MainMenu, Hud, GameOver, Registry, Highscore, Credits, Permanent, Settings, OnlineHighscore }
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
            {OverlayEnum.Settings, SettingsOverlay = GetComponentInChildren<SettingsOverlay>(true)},
            {OverlayEnum.GameOver, GetComponentInChildren<GameOverOverlay>(true)},
            {OverlayEnum.Registry, RegistryOverlay = GetComponentInChildren<RegistryOverlay>(true)},
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
}