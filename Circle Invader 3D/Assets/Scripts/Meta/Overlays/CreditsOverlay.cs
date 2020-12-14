using UnityEngine;

public class CreditsOverlay : MenuOverlay
{
    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(-0.9f, 2.27f, -3), new Vector3(90, 0, 135));
    }
}