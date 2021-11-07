using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.BigHammer")]
public class MultiHammer : Powerup
{
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().BarrierManager.RepairAllBarriers(1);
    }
}