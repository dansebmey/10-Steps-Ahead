using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistryOverlay : Overlay
{
    private CustomTextField[] _customTextFields;
    public CustomTextField HighlightedField { get; private set; }

    private int _highlightedFieldIndex = 0;

    private Button _submitButton;
    private Text _submitButtonText;
    private string[] LoadingTextVars => OnlineHighscoreManager.LoadingTextVars;

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
        
    }

    public override void OnShow()
    {
        Gm.SwitchState(typeof(RegistryState));
        HighlightSelectedField();
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
        Gm.OnlineHighscoreManager.UploadNewHighscore(DetermineUsername(), Gm.PlayerScore);
        _submitButton.interactable = false;
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

    public void ShowUploadResult(OnlineHighscoreManager ohsm)
    {
        StartCoroutine(_ShowUploadResult(ohsm));
    }

    private IEnumerator _ShowUploadResult(OnlineHighscoreManager ohsManager)
    {
        while (ohsManager.uploadProgressCode == OnlineHighscoreManager.IN_PROGRESS)
        {
            yield return new WaitForSeconds(0.25f);
        }

        switch (ohsManager.uploadProgressCode)
        {
            case OnlineHighscoreManager.SUCCEEDED:
                Gm.OverlayManager.SetActiveOverlay(OverlayManager.OverlayEnum.Highscore);
                break;
            case OnlineHighscoreManager.FAILED:
                _submitButton.interactable = true;
                notificationText.text = "Upload to global highscores failed :(";
                notificationText.color = Gm.AestheticsManager.veryBadColor;
                _submitButtonText.text = "Retry";
                break;
        }

        ohsManager.uploadProgressCode = OnlineHighscoreManager.NOT_UPLOADING;
    }
}