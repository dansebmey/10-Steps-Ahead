public abstract class MenuOverlay : Overlay
{
    public override void OnShow()
    {
        Gm.SwitchState(typeof(MenuState));
    }
}