using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AestheticsManager : MonoBehaviour
{
    

    [Header("Colours")]
    public Color specialColor;
    public Color goodColor;
    public Color neutralColor;
    public Color badColor;
    public Color veryBadColor;

    [Header("Fonts")]
    public Font defaultHeaderFont;
    public Font defaultTextFont;
    public int defaultFontSize;
    
    public Font dyslexicHeaderFont;
    public Font dyslexicTextFont;
    public int dyslexicFontSize;

    private Text[] _allTextObjects;
    private List<Text> _buttonTexts;

    private void Awake()
    {
        _allTextObjects = FindObjectsOfType<Text>(true);
        
        _buttonTexts = new List<Text>();
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button b in buttons)
        {
            Text buttonText = b.GetComponentInChildren<Text>(true);
            if (buttonText != null)
            {
                _buttonTexts.Add(buttonText);   
            }
        }
    }

    private bool _isDyslexicFontShown;
    public bool IsDyslexicFontShown
    {
        get => _isDyslexicFontShown;
        set
        {
            _isDyslexicFontShown = value;
            
            if (_isDyslexicFontShown)
            {
                foreach (Text text in _allTextObjects)
                {
                    if (!text.CompareTag("FixedFontText"))
                    {
                        text.font = text.CompareTag("HeaderText") ? dyslexicHeaderFont : dyslexicTextFont;
                    }
                }
                foreach (Text text in _buttonTexts)
                {
                    text.fontSize = dyslexicFontSize;
                }
            }
            else
            {
                foreach (Text text in _allTextObjects)
                {
                    if (!text.CompareTag("FixedFontText"))
                    {
                        text.font = text.CompareTag("HeaderText") ? defaultHeaderFont : defaultTextFont;
                    }
                }
                foreach (Text text in _buttonTexts)
                {
                    text.fontSize = defaultFontSize;
                }
            }
        }
    }
    
    public void ToggleFont()
    {
        IsDyslexicFontShown = !IsDyslexicFontShown;
    }
}