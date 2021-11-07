using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementProgressPanel : GmAwareObject, IPlayerCommandListener, IResetOnGameStart
{
    private Animator _animator;
    private List<AchievementEntryPinned> entryContainers;

    private List<Achievement> pinnedAchievements;

    protected override void Awake()
    {
        base.Awake();
        
        _animator = GetComponent<Animator>();
        entryContainers = GetComponentsInChildren<AchievementEntryPinned>(true).ToList();
    }

    private void Start()
    {
        pinnedAchievements = new List<Achievement>();
    }

    public void ToggleAchievementPin(Achievement achievement, bool doPin, bool animateEntry)
    {
        if (doPin)
        {
            AchievementEntryPinned assignedContainer = entryContainers.First(e => !e.gameObject.activeSelf);
            assignedContainer.AssignTo(achievement);
            assignedContainer.gameObject.SetActive(true);
            assignedContainer.Appear();
            pinnedAchievements.Add(achievement);

            if (pinnedAchievements.Count == entryContainers.Count)
            {
                EventManager<bool, bool>.Invoke(EventType.TogglePinButtons, false, false);
            }
        }
        else
        {
            AchievementEntryPinned containerToClear = entryContainers.First(e => e.achievement == achievement);
            if (animateEntry)
            {
                containerToClear.Disappear();
            }
            else
            {
                containerToClear.gameObject.SetActive(false);
            }
            pinnedAchievements.Remove(achievement);

            EventManager<bool, bool>.Invoke(EventType.TogglePinButtons, true, true);
        }
    }

    public int GetPinnedAchievementCount()
    {
        return pinnedAchievements.Count;
    }

    public void Appear()
    {
        _animator.Play("achievement-panel-appear");
        StartCoroutine(ShowPinnedAchievements());
    }

    private IEnumerator ShowPinnedAchievements()
    {
        foreach (AchievementEntryPinned entry in entryContainers)
        {
            entry.Appear();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Disappear()
    {
        _animator.Play("achievement-panel-disappear");
    }

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        foreach (AchievementEntryPinned entry in entryContainers)
        {
            entry.UpdateProgress();
            entry.UpdatePinButton();
        }
    }

    public void OnNewGameStart()
    {
        foreach (AchievementEntryPinned entry in entryContainers)
        {
            entry.UpdateProgress();
        }
    }

    public void OnGameLoad(GameData gameData)
    {
        // throw new NotImplementedException();
    }
}
