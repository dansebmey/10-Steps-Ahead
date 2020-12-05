using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Hammer")]
public class Hammer : Powerup
{
    public int range;
    public int healValue;
    
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().BarrierManager.RepairBarriers(range, healValue);
    }
}