using UnityEngine;
using UnityEngine.UI;

public class AestheticsManager : MonoBehaviour
{
    private bool _isDyslexicFontShown;
    public bool IsDyslexicFontShown
    {
        get => _isDyslexicFontShown;
        set
        {
            _isDyslexicFontShown = value;
            Text[] allTextObjects = FindObjectsOfType<Text>(true);
        
            if (_isDyslexicFontShown)
            {
                foreach (Text text in allTextObjects)
                {
                    if (!text.CompareTag("FixedFontText"))
                    {
                        text.font = text.CompareTag("HeaderText") ? dyslexicHeaderFont : dyslexicTextFont;
                    }
                }
            }
            else
            {
                foreach (Text text in allTextObjects)
                {
                    if (!text.CompareTag("FixedFontText"))
                    {
                        text.font = text.CompareTag("HeaderText") ? headerFont : textFont;
                    }
                }
            }
        }
    }
    
    public Font headerFont;
    public Font textFont;
    
    public Font dyslexicHeaderFont;
    public Font dyslexicTextFont;

    public Color specialColor;
    public Color goodColor;
    public Color neutralColor;
    public Color badColor;
    public Color veryBadColor;

    public void ToggleFont()
    {
        IsDyslexicFontShown = !IsDyslexicFontShown;
    }
}