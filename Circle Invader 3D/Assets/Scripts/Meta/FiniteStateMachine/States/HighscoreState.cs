using System;
using UnityEngine;

public class HighscoreState : State
{
    private HighscoreOverlay _highscoreOverlay;

    private void Awake()
    {
        throw new NotImplementedException();
    }

    public override void OnEnter()
    {
        Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.HIGHSCORE);
        _highscoreOverlay = Gm.OverlayManager.GetOverlay(OverlayManager.OverlayEnum.HIGHSCORE) as HighscoreOverlay;
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            _highscoreOverlay.HighlightedField.NextCharacter();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            _highscoreOverlay.HighlightedField.PreviousCharacter();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            _highscoreOverlay.HighlightPreviousField();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            _highscoreOverlay.HighlightNextField();
        }
    }

    public override void OnExit()
    {
        
    }
}