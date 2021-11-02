using UnityEngine;

[CreateAssetMenu(menuName = "Item.AntiHammer")]
public class Mine : Item
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        
        gm.BarrierManager.DamageBarrier(4, gm.CurrentPosIndex-1);
        gm.BarrierManager.DamageBarrier(4, gm.CurrentPosIndex);
        gm.BarrierManager.DamageBarrier(4, gm.CurrentPosIndex+1);
        
        gm.AudioManager.Play("MineTriggered");
        gm.AudioManager.Play("BasicAttack");
    }
}