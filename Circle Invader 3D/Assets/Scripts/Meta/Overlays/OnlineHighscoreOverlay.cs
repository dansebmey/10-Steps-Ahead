﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineHighscoreOverlay : MenuOverlay
{
    [SerializeField] private HighscoreEntryController entryControllerPrefab;
    [SerializeField] private Color firstPlaceColour, secondPlaceColour;

    private OnlineHighscoreManager _onlineHighscoreManager;
    private List<HighscoreEntryController> _controllers;

    private Button _refreshButton;
    private Text _loadingText;
    private string[] LoadingTextVars => OnlineHighscoreManager.LoadingTextVars;
    private int _downloadWaitIterations;

    protected override void Awake()
    {
        base.Awake();

        _onlineHighscoreManager = FindObjectOfType<OnlineHighscoreManager>();
        _refreshButton = GetComponentInChildren<Button>();
        _loadingText = GetComponentInChildren<Text>();

        _controllers = new List<HighscoreEntryController>();
        for (int i = 0; i < _onlineHighscoreManager.maxAmtOfEntries; i++)
        {
            HighscoreEntryController controller =
                Instantiate(entryControllerPrefab, new Vector3(0, -144 + (i * -72)), Quaternion.identity);
            controller.transform.SetParent(transform, false);
            controller.gameObject.SetActive(false);

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

        HandleHighscoreSyncing(true);
    }

    public void RefreshOnlineHighscores()
    {
        HandleHighscoreSyncing(true);
        _refreshButton.interactable = false;
    }
    
    private void HandleHighscoreSyncing(bool startNewDownload)
    {
        if (startNewDownload)
        {
            foreach (HighscoreEntryController controller in _controllers)
            {
                controller.gameObject.SetActive(false);
            }
            _loadingText.gameObject.SetActive(true);
            
            StartCoroutine(_HandleHighscoreSyncing());
        }
    }
    
    private IEnumerator _HandleHighscoreSyncing()
    {
        _onlineHighscoreManager.RetryUploadCachedOfflineEntries();
        while (_onlineHighscoreManager.pendingUploadEntries.Count > 0)
        {
            if (!HandleLoadingText("Uploading offline entries")) break;
            yield return new WaitForSeconds(0.25f);
        }
        _downloadWaitIterations = 0;

        if (_onlineHighscoreManager.pendingUploadEntries.Count == 0)
        {
            _onlineHighscoreManager.cachedOnlineEntries = null;
            _onlineHighscoreManager.DownloadHighscores();
        
            while (_onlineHighscoreManager.cachedOnlineEntries == null)
            {
                if (!HandleLoadingText("Downloading global highscores")) break;
                yield return new WaitForSeconds(0.25f);
            }
            _downloadWaitIterations = 0;

            if (_onlineHighscoreManager.cachedOnlineEntries != null)
            {
                _loadingText.gameObject.SetActive(false);
    
                if (_onlineHighscoreManager.cachedOnlineEntries != null)
                {
                    for (int i = 0; i < _controllers.Count; i++)
                    {
                        _controllers[i].gameObject.SetActive(true);
                    
                        bool entryExists = i < _onlineHighscoreManager.cachedOnlineEntries.Length;
                        _controllers[i].SetValues(i + 1, 
                            entryExists ? _onlineHighscoreManager.cachedOnlineEntries[i].username : "", 
                            entryExists ? _onlineHighscoreManager.cachedOnlineEntries[i].score : 0);
                    }
                }
            }
        }
    }

    private bool HandleLoadingText(string baseText)
    {
        _downloadWaitIterations++;
        if (_downloadWaitIterations == 40)
        {
            _loadingText.text = "Could not retrieve global highscore data :(";
            OnDownloadComplete();
            return false;
        }
        
        _loadingText.text = baseText + OnlineHighscoreManager.LoadingTextVars[_downloadWaitIterations % LoadingTextVars.Length];
        return true;
    }

    public void OnDownloadComplete()
    {
        _refreshButton.interactable = true;
    }
}