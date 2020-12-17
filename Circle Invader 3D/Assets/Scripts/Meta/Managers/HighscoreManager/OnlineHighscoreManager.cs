using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineHighscoreManager : HighscoreManager
{
    private OnlineHighscoreOverlay _ohsOverlay;
    private RegistryOverlay _registryOverlay;
    
    private const string privateCode = "yvIHXBo13k-G6t3dmcjRHQjkLmW2idX0qETqYsFTx1Vg";
    private const string publicCode = "5fda77aceb36c70af8369f2f";
    private const string webUrl = "http://dreamlo.com/lb/";

    public const int NOT_UPLOADING = -1;
    public const int IN_PROGRESS = 0;
    public const int FAILED = 1;
    public const int SUCCEEDED = 2;
    public int uploadProgressCode = NOT_UPLOADING;

    [HideInInspector] public HighscoreData.HighscoreEntryData[] cachedOnlineEntries;
    [HideInInspector] public List<HighscoreData.HighscoreEntryData> pendingUploadEntries;

    public static string[] LoadingTextVars;

    protected override void Awake()
    {
        base.Awake();

        _ohsOverlay = FindObjectOfType<OnlineHighscoreOverlay>(true);
        _registryOverlay = FindObjectOfType<RegistryOverlay>(true);
        
        pendingUploadEntries = new List<HighscoreData.HighscoreEntryData>();
        // this is overwritten if a list exists in saved highscore data
    }

    protected override void Start()
    {
        base.Start();

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

    public void UploadNewHighscore(string username, int score)
    {
        uploadProgressCode = IN_PROGRESS;
        StartCoroutine(_UploadNewHighscore(username, score));
    }

    public void RefreshOnlineHighscores()
    {
        _ohsOverlay.RefreshOnlineHighscores();
    }
    
    public void DownloadHighscores()
    {
        StartCoroutine(_DownloadHighscores());
    }
    
    private IEnumerator _UploadNewHighscore(string username, int score)
    {
        UnityWebRequest req = new UnityWebRequest
            (webUrl + privateCode + "/add/" + UnityWebRequest.EscapeURL(username) + "/" + score);
        yield return req.SendWebRequest();

        if (!string.IsNullOrEmpty(req.error))
        {
            pendingUploadEntries.Add(new HighscoreData.HighscoreEntryData(username, score));
        }
        Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Highscore);
    }
    
    public void RetryUploadCachedOfflineEntries()
    {
        if (pendingUploadEntries != null)
        {
            foreach (HighscoreData.HighscoreEntryData entry in pendingUploadEntries)
            {
                UploadNewHighscore(entry.username, entry.score);
                pendingUploadEntries.Remove(entry);
            }
        }
    }
    
    private IEnumerator _DownloadHighscores()
    {
        UnityWebRequest req = new UnityWebRequest(webUrl + publicCode + "/quote/" + maxAmtOfEntries);
        DownloadHandlerBuffer dhb = new DownloadHandlerBuffer();
        req.downloadHandler = dhb;
        
        yield return req.SendWebRequest();

        if (string.IsNullOrEmpty(req.error))
        {
            yield return req.downloadHandler.text;
            cachedOnlineEntries = ConvertTextToGlobalHighscoreEntries(req.downloadHandler.text);
            _ohsOverlay.OnDownloadComplete();
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
}