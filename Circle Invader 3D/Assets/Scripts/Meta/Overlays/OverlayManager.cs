using System;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : GmAwareObject
{
    public HudOverlay Hud { get; private set; }
    public PermaOverlay PermaOverlay { get; private set; }
    
    private Dictionary<OverlayEnum, Overlay> _overlays;
    private Overlay _activeOverlay;
    public enum OverlayEnum { MainMenu, Hud, GameOver, Registry, Highscore, Credits, Permanent, Settings }
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
            {OverlayEnum.MainMenu, GetComponentInChildren<MainMenuOverlay>(true)},
            {OverlayEnum.Hud, Hud = GetComponentInChildren<HudOverlay>(true)},
            {OverlayEnum.Settings, GetComponentInChildren<SettingsOverlay>(true)},
            {OverlayEnum.GameOver, GetComponentInChildren<GameOverOverlay>(true)},
            {OverlayEnum.Registry, GetComponentInChildren<RegistryOverlay>(true)},
            {OverlayEnum.Highscore, GetComponentInChildren<HighscoreOverlay>(true)},
            {OverlayEnum.Credits, GetComponentInChildren<CreditsOverlay>(true)},
            {OverlayEnum.Permanent, PermaOverlay = GetComponentInChildren<PermaOverlay>(true)}
        };
    }

    private void Start()
    {
        foreach (Overlay overlay in _overlays.Values)
        {
            overlay.gameObject.SetActive(false);
        }
        _overlays[OverlayEnum.Permanent].gameObject.SetActive(true);
        _overlays[OverlayEnum.MainMenu].gameObject.SetActive(true);
        SetActiveOverlay(OverlayEnum.MainMenu);
    }
}