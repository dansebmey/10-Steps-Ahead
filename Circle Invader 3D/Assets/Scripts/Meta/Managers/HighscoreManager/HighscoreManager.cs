using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighscoreManager : GmAwareObject
{
    private const string FILENAME = "highscores.tsa";

    private HighscoreData _highscoreData;
    [SerializeField] private int maxAmtOfEntries;

    private void Start()
    {
        _highscoreData = Load() ?? new HighscoreData(new HighscoreData.HighscoreEntry[maxAmtOfEntries]);
    }

    #region Save and load functionality
    private void Save(HighscoreData highscoreData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + FILENAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, highscoreData);
        stream.Close();
    }
    
    private HighscoreData Load()
    {
        string path = Application.persistentDataPath + "/" + FILENAME;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            HighscoreData highscoreData = formatter.Deserialize(stream) as HighscoreData;
            stream.Close();

            return highscoreData;
        }
        
        Debug.LogWarning("Highscore file not found in [" + path + "]");
        return null;
    }

    #endregion

    public bool IsEligibleForHighscore(int score)
    {
        return _highscoreData.entries[maxAmtOfEntries-1].score < score;
    }

    public void RegisterHighscore(string playerName, int score)
    {
        List<HighscoreData.HighscoreEntry> entries = new List<HighscoreData.HighscoreEntry>(_highscoreData.entries);       
        entries[maxAmtOfEntries-1] = new HighscoreData.HighscoreEntry(playerName, score);
        
        entries.Sort(new HighscoreSorter());
        _highscoreData = new HighscoreData(entries.ToArray());
        
        Save(_highscoreData);
    }
    
    private class HighscoreSorter : IComparer<HighscoreData.HighscoreEntry> 
    {
        public int Compare(HighscoreData.HighscoreEntry a, HighscoreData.HighscoreEntry b)
        {
            if (a.score > b.score)
            {
                return -1;
            }
            return b.score < a.score ? 1 : 0;
        }
    } 
}