using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class TutorialManager : GmAwareObject, IPlayerCommandListener
{
    private HudOverlay _hud;
    
    public enum InstructionEnum { Movement, ItemUsing, ItemSwapping }
    public List<InstructionEnum> InstructionsAlreadyShown { get; private set; }

    private const string FILENAME = "instructions.tsa";

    protected override void Awake()
    {
        base.Awake();
        _hud = FindObjectOfType<HudOverlay>();
    }

    private void Start()
    {
        InstructionsAlreadyShown = Load() ?? new List<InstructionEnum>();
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + FILENAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, InstructionsAlreadyShown);
        stream.Close();
    }

    public List<InstructionEnum> Load()
    {
        if (InstructionsAlreadyShown != null)
        {
            return InstructionsAlreadyShown;
        }
        
        string path = Application.persistentDataPath + "/" + FILENAME;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<InstructionEnum> shownInstructions = formatter.Deserialize(stream) as List<InstructionEnum>;
            stream.Close();

            return shownInstructions;
        }
    
        Debug.LogWarning("Instructions file not found in [" + path + "]");
        return null;
    }

    public void ShowInstruction(TutorialManager.InstructionEnum instructionEnum, params KeyCode[] disappearTrigger)
    {
        if (!Load().Contains(instructionEnum))
        {
            _hud.TryShowInstruction(instructionEnum, disappearTrigger);
        }
    }

    public bool HasInstructionBeenShown(InstructionEnum instructionEnum)
    {
        return InstructionsAlreadyShown.Contains(instructionEnum);
    }
    
    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        foreach (KeyValuePair<KeyCode[], Instruction> entry in _hud.InstructionsCurrentlyShown)
        {
            foreach (KeyCode storedKeyCode in entry.Key)
            {
                if (keyCode == storedKeyCode)
                {
                    InstructionsAlreadyShown.Add(entry.Value.instructionEnum);
                    Save();

                    _hud.RemoveInstruction(entry);
                }
            }
        }
    }
}