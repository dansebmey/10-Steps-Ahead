using UnityEngine;

public class AchievementEntryPinned : AchievementEntry
{
    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
        
        gameObject.SetActive(false);
    }
    
    public override void TogglePin()
    {
        achievement.isPinned = !achievement.isPinned;
        Gm.OverlayManager.Hud.AchievementProgressPanel.ToggleAchievementPin(achievement, achievement.isPinned, true);
        pinButton.colors = achievement.isPinned ? pinnedColourBlock : unpinnedColourBlock;
    }

    public void Appear()
    {
        pinButton.colors = pinnedColourBlock;
        _animator.Play("achievement-pinned-appear");
    }
    
    public void Disappear()
    {
        pinButton.colors = unpinnedColourBlock;
        _animator.Play("achievement-pinned-disappear");
    }

    public void PostAnimDeactivate()
    {
        gameObject.SetActive(false);
    }
}