using UnityEngine;
using UnityEngine.UI;

public class MainMenuOverlay : MenuOverlay
{
    private Button _continueButton;

    protected override void Awake()
    {
        base.Awake();

        _continueButton = GetComponentsInChildren<Button>(true)[0];
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 1, -10), new Vector3(-15, 0, 0));
        
        _continueButton.gameObject.SetActive(Gm.PlayerScore > 0 && !Gm.player.isDefeated);
    }

    public void EnableContinueButton()
    {
        _continueButton.gameObject.SetActive(true);
    }
}