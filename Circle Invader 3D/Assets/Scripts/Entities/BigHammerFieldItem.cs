public class BigHammerFieldItem : FieldItem
{
    public override void OnPickup()
    {
        base.OnPickup();
        Gm.OverlayManager.Hud.UpdateBigHammerInterface();
    }

    protected override void Destroy()
    {
        base.Destroy();
        Gm.OverlayManager.Hud.UpdateBigHammerInterface();
    }
}