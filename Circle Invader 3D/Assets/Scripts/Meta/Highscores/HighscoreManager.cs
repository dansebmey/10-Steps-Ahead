using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighscoreManager : GmAwareObject
{
    private const string FILENAME = "highscores.tsa";

    protected HighscoreData HighscoreData { get; private set; }
    public int maxAmtOfEntries;

    protected virtual void Start()
    {
        HighscoreData = Load() ?? new HighscoreData(Gm, new HighscoreData.HighscoreEntryData[maxAmtOfEntries]);
    }

    #region Save and load functionality
    protected void Save(HighscoreData highscoreData)
    {
        if (!Gm.IOEnabled) return;
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + FILENAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, highscoreData);
        stream.Close();
    }

    public HighscoreData Load()
    {
        if (HighscoreData != null)
        {
            return HighscoreData;
        }
        
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
        return HighscoreData.entries[maxAmtOfEntries-1].score < score;
    }

    public void RegisterHighscore(string username, int score)
    {
        List<HighscoreData.HighscoreEntryData> entries = new List<HighscoreData.HighscoreEntryData>(HighscoreData.entries);       
        entries[maxAmtOfEntries-1] = new HighscoreData.HighscoreEntryData(username, score);
        
        entries.Sort(new HighscoreSorter());
        HighscoreData = new HighscoreData(Gm, entries.ToArray());
        
        Save(HighscoreData);
    }
    
    private class HighscoreSorter : IComparer<HighscoreData.HighscoreEntryData> 
    {
        public int Compare(HighscoreData.HighscoreEntryData a, HighscoreData.HighscoreEntryData b)
        {
            if (a.score > b.score)
            {
                return -1;
            }
            return b.score < a.score ? 1 : 0;
        }
    }

    public bool HighscoresExist()
    {
        return HighscoreData != null;
    }
}