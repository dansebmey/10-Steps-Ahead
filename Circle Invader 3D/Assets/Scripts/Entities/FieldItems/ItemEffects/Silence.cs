using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Silence")]
public class Silence : Powerup
{
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().enemy.SilenceLayers(10);
    }
}