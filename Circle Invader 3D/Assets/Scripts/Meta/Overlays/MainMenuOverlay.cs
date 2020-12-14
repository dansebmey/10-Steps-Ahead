using UnityEngine;
using UnityEngine.UI;

public class MainMenuOverlay : MenuOverlay
{
    private Button _continueButton, _newGameButton, _highscoresButton;

    protected override void Awake()
    {
        base.Awake();

        // _continueButton = GetComponentsInChildren<Button>()[0];
        _newGameButton = GetComponentsInChildren<Button>()[0];
        _highscoresButton = GetComponentsInChildren<Button>()[1];
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 1, -10), new Vector3(-15, 0, 0));
    }
}