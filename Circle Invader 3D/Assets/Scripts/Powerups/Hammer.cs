using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Hammer")]
public class Hammer : Powerup
{
    public int range;
    public int healValue;
    
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.AudioManager.Play("HammerFix");
        gm.BarrierManager.RepairBarriers(range, healValue);
    }
}