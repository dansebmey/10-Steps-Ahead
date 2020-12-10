using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverOverlay : Overlay
{
    private Text _gameOverLabel, _scoreLabel, _newRecordLabel, _pressRToLabel;

    protected override void Awake()
    {
        base.Awake();
        
        _gameOverLabel = GetComponentsInChildren<Text>()[1];
        _scoreLabel = GetComponentsInChildren<Text>()[2];
        _pressRToLabel = GetComponentsInChildren<Text>()[3];
        _newRecordLabel = GetComponentsInChildren<Text>()[4];
        
        _newRecordLabel.gameObject.SetActive(false);
    }
    
    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        _scoreLabel.text = "You scored " + Gm.PlayerScore + " points!";

        if (Gm.HighscoreManager.IsEligibleForHighscore(Gm.PlayerScore))
        {
            _newRecordLabel.gameObject.SetActive(true);
            _pressRToLabel.text = "Press R to register your score,\nor ESC to return to the main menu";
            _pressRToLabel.text = "Current state = " + Gm.CurrentState;
        }
        else
        {
            _pressRToLabel.text = "Press R to return to the main menu";
        }
    }
}
