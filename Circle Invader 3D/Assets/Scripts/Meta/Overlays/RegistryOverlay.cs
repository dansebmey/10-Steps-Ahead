﻿using System;
using System.Collections.Generic;

public class RegistryOverlay : Overlay
{
    private CustomTextField[] _customTextFields;
    public CustomTextField HighlightedField { get; private set; }

    private int _highlightedFieldIndex = 0;

    private Dictionary<string, string> _profanityFilterMap;

    protected override void Awake()
    {
        base.Awake();

        _customTextFields = GetComponentsInChildren<CustomTextField>();
    }

    private void Start()
    {
        _profanityFilterMap = new Dictionary<string, string>()
        {
            {"PENIS","8===D"}
        };
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        Gm.SwitchState(typeof(RegistryState));
        HighlightSelectedField();
    }

    private void HighlightSelectedField()
    {
        HighlightedField = _customTextFields[_highlightedFieldIndex];
        foreach (CustomTextField ctf in _customTextFields)
        {
            ctf.ShowArrows(ctf == HighlightedField);
        }
    }

    public void PreviousCharacter()
    {
        HighlightedField.PreviousCharacter();
    }

    public void NextCharacter()
    {
        HighlightedField.NextCharacter();
    }

    public void HighlightPreviousField()
    {
        int newIndex = (_customTextFields.Length + _highlightedFieldIndex - 1) % _customTextFields.Length;
        _highlightedFieldIndex = newIndex;
        
        HighlightSelectedField();
    }

    public void HighlightNextField()
    {
        int newIndex = (_customTextFields.Length + _highlightedFieldIndex + 1) % _customTextFields.Length;
        _highlightedFieldIndex = newIndex;
        
        HighlightSelectedField();
    }

    public void RegisterHighscore()
    {
        string result = "";
        foreach (CustomTextField ctf in _customTextFields)
        {
            result += ctf.GetCharacter();
        }
        if (_profanityFilterMap.ContainsKey(result))
        {
            result = _profanityFilterMap[result];
        }
        
        Gm.HighscoreManager.RegisterHighscore(result, Gm.PlayerScore);
        Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Highscore);
    }
}