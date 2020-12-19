using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.BigHammer")]
public class BigHammer : Powerup
{
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().BarrierManager.RepairAllBarriers(1);
    }
}