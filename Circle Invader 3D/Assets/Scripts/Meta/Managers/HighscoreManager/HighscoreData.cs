using System;

[Serializable]
public class HighscoreData
{
    public HighscoreEntry[] entries;

    public HighscoreData(HighscoreEntry[] entries)
    {
        this.entries = entries;
    }

    [Serializable]
    public struct HighscoreEntry
    {
        public string name;
        public int score;

        public HighscoreEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}