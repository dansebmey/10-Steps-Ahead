using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : State
{
    public override void OnEnter()
    {
        FindObjectOfType<GameOverInterface>(true).OnGameOver(Gm.PlayerScore);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public override void OnExit()
    {
        
    }
}