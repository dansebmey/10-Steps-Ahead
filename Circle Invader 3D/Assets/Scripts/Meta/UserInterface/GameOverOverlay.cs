using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverOverlay : MenuOverlay
{
    private Text _scoreLabel, _newRecordLabel, _pressRToLabel;
    private Button _submitScoreButton, _toMainMenuButton;

    protected override void Awake()
    {
        base.Awake();
        
        _scoreLabel = GetComponentsInChildren<Text>()[2];
        
        _submitScoreButton = GetComponentsInChildren<Button>()[0];
        _toMainMenuButton = GetComponentsInChildren<Button>()[1];
    }
    
    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
        
        _scoreLabel.text = "You scored " + Gm.PlayerScore + " points!";

        if (Gm.HighscoreManager.IsEligibleForHighscore(Gm.PlayerScore))
        {
            _scoreLabel.text += "\nNew highscore!";
            
            _submitScoreButton.gameObject.SetActive(true);
            _submitScoreButton.animator.Play("button-draw-attention");
            
            _toMainMenuButton.interactable = false;
            StartCoroutine(MakeMainMenuButtonInteractable(2));
        }
        else
        {   
            _submitScoreButton.gameObject.SetActive(false);
        }
    }

    private IEnumerator MakeMainMenuButtonInteractable(float delay)
    {
        yield return new WaitForSeconds(delay);
        _toMainMenuButton.interactable = true;
    }
}
