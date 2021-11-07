using System;
using UnityEngine;
using UnityEngine.UI;

public class AchievementEntry : GmAwareObject
{
    [HideInInspector] public Achievement achievement;

    private Image _background;
    private Text nameTextObject;
    private Text descriptionTextObject;
    private Slider progressBar;
    protected Button pinButton;

    private Color incompleteBgColour;
    private Color incompleteTextColour;
    [SerializeField] private Color completedBgColour;
    [SerializeField] private Color completedTextColour;

    [Header("Pinned Colour Block")]
    public ColorBlock pinnedColourBlock;
    protected ColorBlock unpinnedColourBlock;

    private static bool _pinningEnabled;

    protected override void Awake()
    {
        base.Awake();
        
        _background = GetComponentInChildren<Image>();
        nameTextObject = GetComponentsInChildren<Text>()[0];
        descriptionTextObject = GetComponentsInChildren<Text>()[1];
        progressBar = GetComponentInChildren<Slider>();
        pinButton = GetComponentInChildren<Button>();
        
        unpinnedColourBlock = pinButton.colors;
        incompleteBgColour = _background.color;
        incompleteTextColour = nameTextObject.color;
        
        EventManager<bool, bool>.AddListener(EventType.TogglePinButtons, TogglePinButtonEnabled);
    }
    
    public void AssignTo(Achievement a)
    {
        achievement = a;
        nameTextObject.text = a.achievementName;
        descriptionTextObject.text = a.achievementDescription;
        progressBar.maxValue = a.argument;
        progressBar.value = 0;
    }

    public void UpdateProgress()
    {
        progressBar.value = achievement.Progression;
        
        bool c = achievement.isCompleted;
        _background.color = c ? completedBgColour : incompleteBgColour;
        nameTextObject.color = c ? completedTextColour : incompleteTextColour;
        descriptionTextObject.color = c ? completedTextColour : incompleteTextColour;
        if (c)
        {
            if (achievement.isPinned)
            {
                TogglePin();
            }
            TogglePinButtonEnabled(false);
            ColorBlock cb = pinButton.colors;
            cb.disabledColor = Color.clear;
            pinButton.colors = cb;
        }
    }

    public void UpdatePinButton()
    {
        pinButton.colors = achievement.isPinned ? pinnedColourBlock : unpinnedColourBlock;
    }

    private void TogglePinButtonEnabled(bool enable, bool _ = false)
    {
        if (!achievement.isPinned)
        {
            pinButton.interactable = enable && !achievement.isCompleted;
        }
    }

    // needs to be public for Inspector access
    public virtual void TogglePin()
    {
        achievement.isPinned = !achievement.isPinned;
        Gm.OverlayManager.Hud.AchievementProgressPanel.ToggleAchievementPin(achievement, achievement.isPinned, false);
        pinButton.colors = achievement.isPinned ? pinnedColourBlock : unpinnedColourBlock;
    }

    private void OnDestroy()
    {
        EventManager<bool, bool>.RemoveListener(EventType.TogglePinButtons, TogglePinButtonEnabled);
    }
}