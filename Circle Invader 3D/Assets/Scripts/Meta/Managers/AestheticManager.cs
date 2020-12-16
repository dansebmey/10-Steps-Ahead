using UnityEngine;
using UnityEngine.UI;

public class AestheticManager : MonoBehaviour
{
    public bool isDyslexicFontShown;
    
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
        isDyslexicFontShown = !isDyslexicFontShown;
        Text[] allTextObjects = FindObjectsOfType<Text>(true);
        
        if (isDyslexicFontShown)
        {
            foreach (Text text in allTextObjects)
            {
                // text.font = text.font == headerFont ? dyslexicHeaderFont : dyslexicTextFont;
                text.font = text.CompareTag("HeaderText") ? dyslexicHeaderFont : dyslexicTextFont;
            }
        }
        else
        {
            foreach (Text text in allTextObjects)
            {
                // text.font = text.font == dyslexicHeaderFont ? headerFont : textFont;
                text.font = text.CompareTag("HeaderText") ? headerFont : textFont;
            }
        }
    }
}