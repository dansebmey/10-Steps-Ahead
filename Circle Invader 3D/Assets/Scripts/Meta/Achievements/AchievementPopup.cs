using System;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopup : MonoBehaviour
{
    public AchievementManager achievementManager;

    private Animator _animator;
    private Text nameTextObject;
    private Text descriptionTextObject;

    [HideInInspector] public bool isShowing;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        nameTextObject = GetComponentsInChildren<Text>()[0];
        descriptionTextObject = GetComponentsInChildren<Text>()[1];
    }

    public void ShowCompletedAchievement(Achievement achievement)
    {
        isShowing = true;
        
        _animator.Play("achievement-appear");
        FindObjectOfType<AudioManager>().Play("achievement-completed");
        nameTextObject.text = achievement.achievementName;
        descriptionTextObject.text = achievement.achievementDescription;
    }

    public void ShowNextQueuedAchievement()
    {
        isShowing = false;
        
        Achievement nextAchievement = achievementManager.GetNextQueuedAchievement();
        if (nextAchievement != null)
        {
            ShowCompletedAchievement(nextAchievement);   
        }
    }
}