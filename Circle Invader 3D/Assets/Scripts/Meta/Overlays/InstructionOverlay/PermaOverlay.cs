using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class PermaOverlay : Overlay, IPlayerCommandListener
{
    public enum InstructionEnum { Movement, ItemUsing, ItemSwapping }

    private ConcurrentDictionary<KeyCode[], Instruction> _instructionsCurrentlyShown;

    public List<Instruction> instructionPrefabs;
    private List<InstructionEnum> _instructionsAlreadyShown;

    private const string FILENAME = "instructions.tsa";

    private void Start()
    {
        _instructionsCurrentlyShown = new ConcurrentDictionary<KeyCode[], Instruction>();
        _instructionsAlreadyShown = Load() ?? new List<InstructionEnum>();
    }

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        foreach (KeyValuePair<KeyCode[], Instruction> entry in _instructionsCurrentlyShown)
        {
            foreach (KeyCode storedKeyCode in entry.Key)
            {
                if (keyCode == storedKeyCode)
                {
                    _instructionsAlreadyShown.Add(entry.Value.instructionEnum);
                    
                    Destroy(entry.Value.gameObject);
                    _instructionsCurrentlyShown.TryRemove(entry.Key, out Instruction instruction);
                }
            }
        }
    }

    public void ShowInstruction(InstructionEnum instructionEnum, params KeyCode[] disappearTrigger)
    {
        if (!Load().Contains(instructionEnum))
        {
            foreach (Instruction prefab in instructionPrefabs)
            {
                if (prefab.instructionEnum == instructionEnum)
                {
                    Instruction instruction = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
                    instruction.transform.SetParent(transform, false);
                    _instructionsCurrentlyShown.TryAdd(disappearTrigger, instruction);

                    Save(_instructionsAlreadyShown);
                }
            }
        }
    }

    public bool IsInstructionShown(InstructionEnum instructionEnum)
    {
        return _instructionsAlreadyShown.Contains(instructionEnum);
    }

    private void Save(List<InstructionEnum> shownInstructions)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + FILENAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, shownInstructions);
        stream.Close();
    }

    private List<InstructionEnum> Load()
    {
        if (_instructionsAlreadyShown != null)
        {
            return _instructionsAlreadyShown;
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

    public bool IsNoOtherInstructionShown()
    {
        return _instructionsCurrentlyShown.IsEmpty;
    }
}