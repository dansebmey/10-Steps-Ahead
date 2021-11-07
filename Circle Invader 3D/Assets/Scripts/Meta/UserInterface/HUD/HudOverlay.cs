using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudOverlay : Overlay
{
    private TutorialManager _tutorialManager;
    
    public ConcurrentDictionary<KeyCode[], Instruction> InstructionsCurrentlyShown { get; private set; }
    public List<Instruction> instructionPrefabs;
    
    private Text _scoreShadowLabel, _scoreLabel;

    private BigHammerInterface _bigHammerInterface;
    [HideInInspector] public AchievementProgressPanel AchievementProgressPanel;
    
    public Text clockTextObject;

    protected override void Awake()
    {
        base.Awake();
        
        _scoreShadowLabel = GetComponentsInChildren<Text>(true)[0];
        _scoreLabel = GetComponentsInChildren<Text>(true)[1];

        _bigHammerInterface = GetComponentInChildren<BigHammerInterface>();
        AchievementProgressPanel = GetComponentInChildren<AchievementProgressPanel>();
        
        InstructionsCurrentlyShown = new ConcurrentDictionary<KeyCode[], Instruction>();
    }

    public void UpdateScore(int score)
    {
        _scoreShadowLabel.text = score.ToString();
        _scoreLabel.text = score.ToString();
    }

    public void UpdateBigHammerInterface()
    {
        _bigHammerInterface.UpdateMeter();
    }

    public override void OnHide()
    {
        
    }

    public override void OnShow()
    {
        Gm.CameraController.FocusOn(Gm.player.transform, new Vector3(0, 5, -5), new Vector3(25, 0, 0));
        Gm.SwitchState(typeof(WaitingForPlayerActionState));

        AchievementProgressPanel.Appear();
    }
    public bool IsNoOtherInstructionShown()
    {
        return InstructionsCurrentlyShown.IsEmpty;
    }

    public void TryShowInstruction(TutorialManager.InstructionEnum instructionEnum, KeyCode[] disappearTrigger)
    {
        foreach (Instruction prefab in instructionPrefabs)
        {
            if (prefab.instructionEnum == instructionEnum)
            {
                Instruction instruction = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
                instruction.transform.SetParent(transform, false);
                InstructionsCurrentlyShown.TryAdd(disappearTrigger, instruction);
            }
        }
    }

    public void RemoveInstruction(KeyValuePair<KeyCode[], Instruction> entry)
    {
        Destroy(entry.Value.gameObject);
        InstructionsCurrentlyShown.TryRemove(entry.Key, out Instruction instruction);
    }

    public void ToggleClock()
    {
        clockTextObject.gameObject.SetActive(!clockTextObject.gameObject.activeSelf);
    }

    public void UpdateClock(int secondsPassed)
    {
        int min = secondsPassed / 60;
        int sec = secondsPassed % 60;

        clockTextObject.text = min + ":" + (sec < 10 ? "0" : "") + sec;
    }
}
