using UnityEngine;

[CreateAssetMenu(menuName = "Hammer")]
public class Hammer : Powerup
{
    public int range;
    public int healValue;
    
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().BarrierManager.RepairBarriers(range, healValue);
    }
}