using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Hammer")]
public class Hammer : Powerup
{
    public int range;
    public int healValue;
    
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.AudioManager.Play("HammerFix", 0.05f);
        gm.BarrierManager.RepairBarriers(range, healValue);
    }
}