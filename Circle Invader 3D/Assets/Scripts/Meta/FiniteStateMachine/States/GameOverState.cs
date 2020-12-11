using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : State
{
    private bool _isEligibleForHighscore;
    
    public override void OnEnter()
    {
        Debug.LogError("SHOULD NOT ENTER THIS STATE - REMOVAL PENDING");
    }

    public override void OnUpdate()
    {
        if (_isEligibleForHighscore)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                Gm.SwitchState(typeof(RegistryState));
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                Gm.SwitchState(typeof(MenuState));
                Gm.ToMainMenuPressed();
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                Gm.SwitchState(typeof(MenuState));
            }
        }
    }

    public override void OnExit()
    {
        
    }
}