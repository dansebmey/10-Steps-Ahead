using System;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreEntryController : MonoBehaviour
{
    private Text _rankText, _nameText, _scoreText;

    private void Awake()
    {
        _rankText = GetComponentsInChildren<Text>()[0];
        _nameText = GetComponentsInChildren<Text>()[1];
        _scoreText = GetComponentsInChildren<Text>()[2];
    }

    public void SetValues(int index, HighscoreData.HighscoreEntry entry)
    {
        _rankText.text = index + ".";
        _nameText.text = entry.name;
        _scoreText.text = entry.score.ToString();
    }

    public void SetColor(Color color)
    {
        _rankText.color = color;
        _nameText.color = color;
        _scoreText.color = color;
    }
}