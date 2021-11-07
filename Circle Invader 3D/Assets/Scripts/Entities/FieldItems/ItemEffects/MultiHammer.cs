using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.BigHammer")]
public class MultiHammer : Powerup
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.BarrierManager.RepairAllBarriers(1);
        
        int carriedMultiHammers = gm.player.Inventory.CountAmountOfItem("MultiHammer") - 1;
        EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
            AchievementManager.AchievementType.CarryTwoMultiHammers, carriedMultiHammers);
    }
}