﻿using System.Collections.Generic;
using UnityEngine;

public class HighscoreOverlay : MenuOverlay
{
    [SerializeField] private HighscoreEntryController entryControllerPrefab;
    [SerializeField] private Color firstPlaceColour, secondPlaceColour;

    private HighscoreManager _highscoreManager;
    private List<HighscoreEntryController> _controllers;

    protected override void Awake()
    {
        base.Awake();

        _highscoreManager = FindObjectOfType<HighscoreManager>();
        HighscoreData highscoreData = _highscoreManager.Load();
        
        _controllers = new List<HighscoreEntryController>();
        for (int i = 0; i < _highscoreManager.maxAmtOfEntries; i++)
        {
            HighscoreEntryController controller =
                Instantiate(entryControllerPrefab, new Vector3(0, -144 + (i * -72)), Quaternion.identity);
            controller.transform.SetParent(transform, false);

            if (i == 0)
            {
                controller.SetColor(firstPlaceColour);
            }
            else if (i == 1)
            {
                controller.SetColor(secondPlaceColour);
            }
            
            _controllers.Add(controller);
        }
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
        
        Gm.CameraController.FocusOn(Gm.CameraController.DefaultFocalPoint, new Vector3(0, 7.8f, 0), new Vector3(90, 0, 0));
        
        HighscoreData highscoreData = _highscoreManager.Load();
        if (highscoreData != null)
        {
            for (int i = 0; i < _controllers.Count; i++)
            {
                _controllers[i].SetValues(i + 1, highscoreData.entries[i]);
            }
        }
    }
}