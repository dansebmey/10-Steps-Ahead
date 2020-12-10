using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : State
{
    private bool _isEligibleForHighscore;
    
    public override void OnEnter()
    {
        Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.GAME_OVER);
        _isEligibleForHighscore = Gm.HighscoreManager.IsEligibleForHighscore(Gm.PlayerScore);
    }

    public override void OnUpdate()
    {
        if (_isEligibleForHighscore)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                Gm.SwitchState(typeof(HighscoreState));
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public override void OnExit()
    {
        
    }
}