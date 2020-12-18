using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineHighscoreManager : HighscoreManager
{
    private OnlineHighscoreOverlay _ohsOverlay;
    private RegistryOverlay _registryOverlay;
    
    private const string PRIVATE_CODE = "yvIHXBo13k-G6t3dmcjRHQjkLmW2idX0qETqYsFTx1Vg";
    private const string PUBLIC_CODE = "5fda77aceb36c70af8369f2f";
    private const string WEB_URL = "http://dreamlo.com/lb/";

    [HideInInspector] public HighscoreData.HighscoreEntryData[] cachedOnlineEntries;
    [HideInInspector] public List<HighscoreData.HighscoreEntryData> pendingUploadEntries;

    public static string[] LoadingTextVars;

    protected override void Awake()
    {
        base.Awake();

        _ohsOverlay = FindObjectOfType<OnlineHighscoreOverlay>(true);
        _registryOverlay = FindObjectOfType<RegistryOverlay>(true);

        LoadingTextVars = new[]
        {
            ".    ",
            ". .  ",
            ". . .",
            "  . .",
            "    .",
            "     "
        };
    }

    protected override void Start()
    {
        base.Start();
        
        pendingUploadEntries = new List<HighscoreData.HighscoreEntryData>(
            new List<HighscoreData.HighscoreEntryData>(HighscoreData.pendingUploadEntries));
    }

    public void UploadNewHighscore(string username, int score)
    {
        StartCoroutine(_UploadNewHighscore(new HighscoreData.HighscoreEntryData(username, score)));
    }

    public void RefreshOnlineHighscores()
    {
        _ohsOverlay.RefreshOnlineHighscores();
    }
    
    public void DownloadHighscores()
    {
        StartCoroutine(_DownloadHighscores());
    }
    
    private IEnumerator _UploadNewHighscore(HighscoreData.HighscoreEntryData entry)
    {
        UnityWebRequest req = new UnityWebRequest
            (WEB_URL + PRIVATE_CODE + "/add/" + UnityWebRequest.EscapeURL(entry.username) + "/" + entry.score);
        yield return req.SendWebRequest();

        if (!string.IsNullOrEmpty(req.error) && !pendingUploadEntries.Contains(entry))
        {
            AddPendingUploadEntry(entry);
        }
        else if (string.IsNullOrEmpty(req.error) && pendingUploadEntries.Contains(entry))
        {
            RemovePendingUploadEntry(entry);
        }

        _registryOverlay.OnUploadAttemptComplete();
    }
    
    public void RetryUploadCachedOfflineEntries()
    {
        if (pendingUploadEntries != null)
        {
            foreach (HighscoreData.HighscoreEntryData entry in pendingUploadEntries)
            {
                UploadNewHighscore(entry.username, entry.score);
            }
        }
    }
    
    private IEnumerator _DownloadHighscores()
    {
        UnityWebRequest req = new UnityWebRequest(WEB_URL + PUBLIC_CODE + "/quote/" + maxAmtOfEntries);
        DownloadHandlerBuffer dhb = new DownloadHandlerBuffer();
        req.downloadHandler = dhb;
        
        yield return req.SendWebRequest();

        if (string.IsNullOrEmpty(req.error))
        {
            yield return req.downloadHandler.text;
            cachedOnlineEntries = ConvertTextToGlobalHighscoreEntries(req.downloadHandler.text);
            _ohsOverlay.OnDataTransferComplete();
        }
        else
        {
            yield return "Error";
        }
    }
    
    private HighscoreData.HighscoreEntryData[] ConvertTextToGlobalHighscoreEntries(string text)
    {
        string[] rawEntries = text.Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None);
        HighscoreData.HighscoreEntryData[] result = new HighscoreData.HighscoreEntryData[rawEntries.Length-1];
        for (int i = 0; i < rawEntries.Length-1; i++) // Length-1 because the last entry is always empty
        {
            HighscoreData.HighscoreEntryData entry = new HighscoreData.HighscoreEntryData(
                rawEntries[i].Split('"')[1].Replace('+', ' '),
                int.Parse(rawEntries[i].Split('"')[3]));
            result[i] = entry;
        }
        
        return result;
    }

    public void AddPendingUploadEntry(HighscoreData.HighscoreEntryData entry)
    {
        pendingUploadEntries.Add(entry);
        HighscoreData.pendingUploadEntries = pendingUploadEntries.ToArray();
        Save(HighscoreData);
    }

    private void RemovePendingUploadEntry(HighscoreData.HighscoreEntryData entry)
    {
        pendingUploadEntries.Remove(entry);
        HighscoreData.pendingUploadEntries = pendingUploadEntries.ToArray();
        Save(HighscoreData);
    }
}