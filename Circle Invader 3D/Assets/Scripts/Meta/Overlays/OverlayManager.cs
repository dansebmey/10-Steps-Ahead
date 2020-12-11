using System;
using System.Collections.Generic;

public class OverlayManager : GmAwareObject
{
    public HudOverlay Hud { get; private set; }
    
    private Dictionary<OverlayEnum, Overlay> _overlays;
    private Overlay _activeOverlay;
    public enum OverlayEnum { MAIN_MENU, HUD, GAME_OVER, REGISTRY, HIGHSCORE, CREDITS }
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
            {OverlayEnum.MAIN_MENU, GetComponentInChildren<MainMenuOverlay>(true)},
            {OverlayEnum.HUD, Hud = GetComponentInChildren<HudOverlay>(true)},
            {OverlayEnum.GAME_OVER, GetComponentInChildren<GameOverOverlay>(true)},
            {OverlayEnum.REGISTRY, GetComponentInChildren<RegistryOverlay>(true)},
            {OverlayEnum.HIGHSCORE, GetComponentInChildren<HighscoreOverlay>(true)},
            {OverlayEnum.CREDITS, GetComponentInChildren<CreditsOverlay>(true)}
        };
    }

    private void Start()
    {
        foreach (Overlay overlay in _overlays.Values)
        {
            overlay.gameObject.SetActive(false);
        }
        _overlays[OverlayEnum.MAIN_MENU].gameObject.SetActive(true);
        SetActiveOverlay(OverlayEnum.MAIN_MENU);
    }
}