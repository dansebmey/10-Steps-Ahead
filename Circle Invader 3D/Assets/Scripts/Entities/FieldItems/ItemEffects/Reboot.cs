using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Silence")]
public class Reboot : Powerup
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>(); 
        gm.enemy.SilenceLayers(10);
        gm.AudioManager.Play("shutdown");
    }
}