using System;

[Serializable]
public class HighscoreData
{
    public HighscoreEntryData[] entries;
    public HighscoreEntryData[] pendingUploadEntries;

    public HighscoreData(GameManager gm, HighscoreEntryData[] entries)
    {
        this.entries = entries;
        
        pendingUploadEntries = gm.OnlineHighscoreManager.pendingUploadEntries.ToArray();
    }

    [Serializable]
    public struct HighscoreEntryData
    {
        public string username;
        public int score;

        public HighscoreEntryData(string username, int score)
        {
            this.username = username;
            this.score = score;
        }
    }
}