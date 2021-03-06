﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighscoreEntryState : State
{
    private HighscoreEntryOverlay _highscoreEntryOverlay;
    private Dictionary<KeyCode, Action> _actionMapping;

    public override void OnEnter()
    {
        _highscoreEntryOverlay = Gm.OverlayManager.GetOverlay(OverlayManager.OverlayEnum.Registry) as HighscoreEntryOverlay;

        _actionMapping = new Dictionary<KeyCode, Action>
        {
            {KeyCode.W, _highscoreEntryOverlay.NextCharacter},
            {KeyCode.S, _highscoreEntryOverlay.PreviousCharacter},
            {KeyCode.A, _highscoreEntryOverlay.HighlightPreviousField},
            {KeyCode.D, _highscoreEntryOverlay.HighlightNextField}
            // {KeyCode.Return, RegisterHighscore},
            // {KeyCode.Escape, ReturnToMainMenu}
        };
    }

    public override void OnUpdate()
    {
        foreach (KeyValuePair<KeyCode, Action> entry in _actionMapping)
        {
            if (Input.GetKeyUp(entry.Key))
            {
                Gm.AudioManager.Play("ButtonHover");
                entry.Value.Invoke();
                // StartCoroutine(RepeatKeyPress(entry));
            }
        }
    }

    private IEnumerator RepeatKeyPress(KeyValuePair<KeyCode, Action> entry)
    {
        yield return new WaitForSeconds(0.25f);
        while (Input.GetKey(entry.Key))
        {
            entry.Value.Invoke();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void OnExit()
    {
        
    }
}