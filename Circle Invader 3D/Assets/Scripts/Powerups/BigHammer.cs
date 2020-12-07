using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.BigHammer")]
public class BigHammer : Powerup
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.BarrierManager.RepairAllBarriers(1);
    }
}