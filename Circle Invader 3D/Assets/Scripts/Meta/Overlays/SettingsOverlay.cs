using UnityEngine;

public class SettingsOverlay : MenuOverlay
{
    public override void OnShow()
    {
        base.OnShow();
        
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 10, 0), new Vector3(90, 180, 0));
    }
}