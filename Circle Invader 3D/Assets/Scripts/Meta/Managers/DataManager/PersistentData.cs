using System;

[Serializable]
public class PersistentData
{
    public string lastEnteredHighscoreName;
    public float musicVolume;
    public float sfxVolume;
    public bool isDyslexicFontShown;
    
    public PersistentData(GameManager gm)
    {
        lastEnteredHighscoreName = ((RegistryOverlay)gm.OverlayManager.GetOverlay(OverlayManager.OverlayEnum.Registry)).LastEnteredName;
        musicVolume = gm.AudioManager.MusicVolume;
        sfxVolume = gm.AudioManager.SfxVolume;
        isDyslexicFontShown = gm.AestheticsManager.IsDyslexicFontShown;
    }
}