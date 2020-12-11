using UnityEngine;

public class CreditsOverlay : MenuOverlay
{
    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 7.8f, 0), new Vector3(180, 0, 0));
    }
}