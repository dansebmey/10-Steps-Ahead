public class BigHammerFieldItem : FieldItem
{
    public override void OnPickup()
    {
        if (Gm.player.Inventory.Contains("MultiHammer"))
        {
            EventManager<AchievementManager.AchievementType, int>.Invoke(EventType.SetAchievementProgress,
                AchievementManager.AchievementType.CarryTwoMultiHammers, 2);
        }
        base.OnPickup();
        Gm.OverlayManager.Hud.UpdateBigHammerInterface();
    }

    public override void Destroy()
    {
        base.Destroy();
        Gm.OverlayManager.Hud.UpdateBigHammerInterface();
    }
}