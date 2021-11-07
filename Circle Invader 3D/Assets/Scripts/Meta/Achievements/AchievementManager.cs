using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour, IResetOnGameStart
{
    [SerializeField] private AchievementPopup _popup;

    public List<Achievement> achievements;
    private Queue<Achievement> _achievementsToShow;
    
    public enum AchievementType
    {
        ScoreMilestone, PointsWithoutCollapse, PointsWithoutPowerups, BarrierHPLost, BarrierHPAbove,
        AllBarriersSameHP, TotalHPOf10Barriers, OptimalHammerUse, OptimalMultiHammerUse, OptimalWarpUse, OptimalRebootUse,
        ConsecutivePowerupSwaps, CarryTwoMultiHammers, AvoidDelayedDamage, AvoidSplitDamage, ReduceDoubleDamage,
        PreventMineSpawning, PerfectEverything, Speedrun, ReduceSplitDamage
    }
    public enum AchievementTier { Easy, Medium, Hard }

    private readonly AchievementType[] perfectionAchievements = new[]
    {
        AchievementType.OptimalHammerUse, AchievementType.OptimalMultiHammerUse, AchievementType.OptimalWarpUse,
        AchievementType.OptimalRebootUse, AchievementType.AvoidDelayedDamage, AchievementType.ReduceDoubleDamage,
        AchievementType.AvoidSplitDamage
    };

    private void Awake()
    {
        EventManager<AchievementType, int>.AddListener(EventType.IncrementAchievementProgress, IncrementAchievementProgress);
        EventManager<AchievementType, int>.AddListener(EventType.SetAchievementProgress, SetAchievementProgress);
        EventManager<AchievementType, int>.AddListener(EventType.EnableStepCounter, StartCountingSteps);
        EventManager<AchievementType, int>.AddListener(EventType.ResetAchievementProgress, ResetAchievementProgress);
    }

    private void Start()
    {
        _achievementsToShow = new Queue<Achievement>();
        StartCountingSteps(AchievementType.PointsWithoutCollapse, 0);
        StartCountingSteps(AchievementType.Speedrun, 0);
        StartCountingSteps(AchievementType.PerfectEverything, 0);
    }

    private void IncrementAchievementProgress(AchievementType type, int increment)
    {
        foreach (Achievement a in achievements)
        {
            if (a.type == type && !a.isCompleted)
            {
                a.Progression += increment;

                if (a.isCompleted)
                {
                    QueueAchievementToShow(a);
                }
            }
        }
    }

    private void SetAchievementProgress(AchievementType type, int value)
    {
        foreach (Achievement a in achievements)
        {
            if (a.type == type && !a.isCompleted)
            {
                a.Progression = value;

                if (a.isCompleted)
                {
                    QueueAchievementToShow(a);
                }
            }
        }
    }

    private void StartCountingSteps(AchievementType type, int score)
    {
        foreach (Achievement a in achievements)
        {
            if (a.type == type && !a.isCompleted)
            {
                a.isStepCounterEnabled = true;
            }
        }
    }

    public void IncrementStepCounter()
    {
        foreach (Achievement a in achievements)
        {
            if (a.countSteps && a.isStepCounterEnabled && !a.isCompleted)
            {
                a.Progression++;
                
                if (a.isCompleted)
                {
                    QueueAchievementToShow(a);
                }
            }
        }
    }

    public void QueueAchievementToShow(Achievement achievement)
    {
        _achievementsToShow.Enqueue(achievement);
        if (!_popup.isShowing)
        {
            _popup.ShowCompletedAchievement(_achievementsToShow.Dequeue());   
        }
    }

    public Achievement GetNextQueuedAchievement()
    {
        return _achievementsToShow.Count > 0 ? _achievementsToShow.Dequeue() : null;
    }

    private void ResetAchievementProgress(AchievementType type, int _)
    {
        foreach (Achievement a in achievements)
        {
            if (a.type == type && !a.isCompleted)
            {
                a.Progression = 0;
                a.isStepCounterEnabled = false;
            }
        }

        if (perfectionAchievements.Contains(type))
        {
            ResetAchievementProgress(AchievementType.PerfectEverything, 0);
        }
    }

    private void OnDestroy()
    {
        EventManager<AchievementType, int>.RemoveListener(EventType.IncrementAchievementProgress, IncrementAchievementProgress);
        EventManager<AchievementType, int>.RemoveListener(EventType.SetAchievementProgress, SetAchievementProgress);
        EventManager<AchievementType, int>.RemoveListener(EventType.ResetAchievementProgress, ResetAchievementProgress);
    }

    public void OnNewGameStart()
    {
        foreach (Achievement a in achievements)
        {
            if (!a.isCompleted)
            {
                a.Progression = 0;
                a.isStepCounterEnabled = false;
            }
        }
    }

    public void OnGameLoad(GameData gameData)
    {
        // throw new NotImplementedException();
    }
}