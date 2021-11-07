using System;
using UnityEngine;
using UnityEngine.UI;

public class AchievementEntry : MonoBehaviour
{
    private Achievement _achievement;

    private Image _background;
    private Text nameTextObject;
    private Text descriptionTextObject;
    private Slider progressBar;

    [SerializeField] private Color completedBgColour;
    [SerializeField] private Color completedTextColour;

    private void Awake()
    {
        _background = GetComponentInChildren<Image>();
        nameTextObject = GetComponentsInChildren<Text>()[0];
        descriptionTextObject = GetComponentsInChildren<Text>()[1];
        progressBar = GetComponentInChildren<Slider>();
    }

    public void AssignTo(Achievement achievement)
    {
        _achievement = achievement;
        nameTextObject.text = achievement.achievementName;
        descriptionTextObject.text = achievement.achievementDescription;
        progressBar.maxValue = achievement.argument;
        progressBar.value = 0;
    }

    public void UpdateProgress()
    {
        progressBar.value = _achievement.Progression;
        if (_achievement.isCompleted)
        {
            _background.color = completedBgColour;
            nameTextObject.color = completedTextColour;
            descriptionTextObject.color = completedTextColour;
        }
    }
}