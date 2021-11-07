using System;
using UnityEngine;

[Serializable]
public class Achievement
{
    public string achievementName;
    [TextArea] public string achievementDescription;
    public AchievementManager.AchievementType type;
    public AchievementManager.AchievementTier tier;
    public int argument;
    
    public bool countSteps;
    [HideInInspector] public bool isStepCounterEnabled;
    
    private int _progression;
    [HideInInspector] public bool isCompleted;

    public int Progression
    {
        get => _progression;
        set
        {
            _progression = value;

            if (_progression >= argument)
            {
                isCompleted = true;
            }
        }
    }
}