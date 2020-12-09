using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : GmAwareObject
{
    private CameraController _cameraController;

    protected override void Awake()
    {
        base.Awake();
        _cameraController = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _cameraController.ZoomOut();
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            _cameraController.ResetZoom();
        }
        else if (Input.GetKeyUp(KeyCode.O))
        {
            Gm.SaveGame();
        }
        else if (Input.GetKeyUp(KeyCode.P))
        {
            Gm.LoadGame();
        }
    }
}
