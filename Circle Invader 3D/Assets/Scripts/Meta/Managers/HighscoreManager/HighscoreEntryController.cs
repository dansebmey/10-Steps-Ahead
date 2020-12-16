using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreEntryController : GmAwareObject
{
    private Text _rankText, _nameText, _scoreText;
    private Text[] _texts;

    protected override void Awake()
    {
        base.Awake();
        
        _texts = new[]
        {
            _rankText = GetComponentsInChildren<Text>()[0],
            _nameText = GetComponentsInChildren<Text>()[1],
            _scoreText = GetComponentsInChildren<Text>()[2]
        };
    }

    private void Start()
    {
        if (Gm.AestheticsManager.IsDyslexicFontShown)
        {
            foreach (Text text in _texts)
            {
                text.font = Gm.AestheticsManager.dyslexicTextFont;
            }
        }
    }

    public void SetValues(int index, HighscoreData.HighscoreEntry entry)
    {
        _rankText.text = index + ".";
        _nameText.text = entry.name;
        _scoreText.text = entry.score.ToString();
    }

    public void SetColor(Color colour)
    {
        foreach (Text text in _texts)
        {
            text.color = colour;
        }
    }
}