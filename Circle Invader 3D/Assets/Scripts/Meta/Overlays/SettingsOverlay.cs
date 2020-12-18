using UnityEngine;
using UnityEngine.UI;

public class SettingsOverlay : MenuOverlay
{
    private Slider _musicVolumeSlider, _sfxVolumeSlider;

    protected override void Awake()
    {
        base.Awake();
        _musicVolumeSlider = GetComponentsInChildren<Slider>(true)[0];
        _sfxVolumeSlider = GetComponentsInChildren<Slider>(true)[1];
    }

    public override void OnHide()
    {
        base.OnHide();
        
        Gm.OverlayManager.UpdateVolumeSliders(_musicVolumeSlider.value, _sfxVolumeSlider.value);
    }

    public override void OnShow()
    {
        base.OnShow();
        
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 10, 0), new Vector3(90, 180, 0));
    }
    
    public void SetMusicVolumeSliderValue(float value)
    {
        _musicVolumeSlider.value = value;
    }

    public void SetSfxVolumeSliderValue(float value)
    {
        _sfxVolumeSlider.value = value;
    }
}