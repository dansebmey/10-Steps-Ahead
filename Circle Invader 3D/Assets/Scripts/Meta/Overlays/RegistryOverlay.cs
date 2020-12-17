using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RegistryOverlay : Overlay
{
    private CustomTextField[] _customTextFields;
    public CustomTextField HighlightedField { get; private set; }

    private int _highlightedFieldIndex = 0;

    private bool _internetConnectionTestPassed = true;
    
    private Button _submitButton;
    private Text _submitButtonText;
    private string[] LoadingTextVars => OnlineHighscoreManager.LoadingTextVars;

    private bool _uploadInProgress;
    private int _downloadWaitIterations;

    [SerializeField] private Text notificationText;

    private Dictionary<string, string> _profanityFilterMap;

    protected override void Awake()
    {
        base.Awake();

        _customTextFields = GetComponentsInChildren<CustomTextField>();
        _submitButton = GetComponentInChildren<Button>();
        _submitButtonText = _submitButton.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        _profanityFilterMap = new Dictionary<string, string>()
        {
            {"PENIS","8===D"}
        };
    }

    public override void OnHide()
    {
        _submitButton.interactable = true;
        _submitButtonText.text = "Submit";
    }

    public override void OnShow()
    {
        Gm.SwitchState(typeof(RegistryState));
        HighlightSelectedField();

        StartCoroutine(CheckInternetConnection());
    }

    private IEnumerator CheckInternetConnection()
    {
        UnityWebRequest req = new UnityWebRequest("http://stuffbydaniel.com");
        yield return req.SendWebRequest();
        
        _internetConnectionTestPassed = req.error == null;
    }

    private void HighlightSelectedField()
    {
        HighlightedField = _customTextFields[_highlightedFieldIndex];
        foreach (CustomTextField ctf in _customTextFields)
        {
            ctf.ShowArrows(ctf == HighlightedField);
        }
    }

    public void PreviousCharacter()
    {
        HighlightedField.PreviousCharacter();
    }

    public void NextCharacter()
    {
        HighlightedField.NextCharacter();
    }

    public void HighlightPreviousField()
    {
        int newIndex = (_customTextFields.Length + _highlightedFieldIndex - 1) % _customTextFields.Length;
        _highlightedFieldIndex = newIndex;
        
        HighlightSelectedField();
    }

    public void HighlightNextField()
    {
        int newIndex = (_customTextFields.Length + _highlightedFieldIndex + 1) % _customTextFields.Length;
        _highlightedFieldIndex = newIndex;
        
        HighlightSelectedField();
    }

    public void RegisterHighscore()
    {
        Gm.HighscoreManager.RegisterHighscore(DetermineUsername(), Gm.PlayerScore);
        if (_internetConnectionTestPassed)
        {
            StartCoroutine(SubmitToGlobalLeaderboard());
        }
        else
        {
            Gm.OnlineHighscoreManager.AddPendingUploadEntry(new HighscoreData.HighscoreEntryData(DetermineUsername(), Gm.PlayerScore));
            Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Highscore);
        }
    }

    private IEnumerator SubmitToGlobalLeaderboard()
    {
        _submitButton.interactable = false;
        Gm.OnlineHighscoreManager.UploadNewHighscore(DetermineUsername(), Gm.PlayerScore);
        
        _uploadInProgress = true;
        while (_uploadInProgress)
        {
            if (!HandleLoadingText("Submitting")) break;
            yield return new WaitForSeconds(0.25f);
        }
        if (!_uploadInProgress)
        {
            Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Highscore);
        }
    }

    private bool HandleLoadingText(string baseText)
    // This method is copied and pasted in multiple classes now. Might be better suited as a static Util method?
    {
        _downloadWaitIterations++;
        if (_downloadWaitIterations >= 40)
        {
            return false;
        }
        
        _submitButtonText.text = baseText + OnlineHighscoreManager.LoadingTextVars[_downloadWaitIterations % LoadingTextVars.Length];
        return true;
    }

    private string DetermineUsername()
    {
        string result = "";
        foreach (CustomTextField ctf in _customTextFields)
        {
            result += ctf.GetCharacter();
        }
        if (_profanityFilterMap.ContainsKey(result))
        {
            result = _profanityFilterMap[result];
        }

        return result;
    }

    public string LastEnteredName
    {
        get
        {
            string result = "";
            foreach (CustomTextField ctf in _customTextFields)
            {
                result += ctf.GetCharacter();
            }

            return result;
        }
    }

    public void SetHighscoreName(string savedName)
    {
        if (savedName.Length != _customTextFields.Length)
        {
            Debug.LogWarning(
                "Saved highscore name differs in length from required length; default name reset");
            return;
        }

        for (int i = 0; i < _customTextFields.Length; i++)
        {
            _customTextFields[i].SetCharacter(savedName.ToCharArray()[i]);
        }
    }

    public void OnUploadAttemptComplete()
    {
        _uploadInProgress = false;
    }
}