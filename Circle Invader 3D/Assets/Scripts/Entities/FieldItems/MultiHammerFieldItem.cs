public class MultiHammerFieldItem : FieldItem
{
    private int carriedMultiHammers;
    
    public override void OnPickup()
    {
        carriedMultiHammers = Gm.player.Inventory.CountAmountOfItem("MultiHammer");
        
        base.OnPickup();
        Gm.OverlayManager.Hud.UpdateBigHammerInterface();

        carriedMultiHammers = Gm.player.Inventory.CountAmountOfItem("MultiHammer");
        EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
            AchievementManager.AchievementType.CarryTwoMultiHammers, carriedMultiHammers);
    }

    public override void Destroy()
    {
        base.Destroy();
        Gm.OverlayManager.Hud.UpdateBigHammerInterface();
    }

    protected override void PostDestroy()
    {
        if (carriedMultiHammers == 2)
        {
            EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
                AchievementManager.AchievementType.CarryTwoMultiHammers, 3);
        }
    }
}