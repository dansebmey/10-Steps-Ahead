using System.IO;
using UnityEngine;

public class DataManager : GmAwareObject
{
    private string file = "savedGame.json";
    
    public void Save()
    {
        string json = JsonUtility.ToJson(Gm);
        WriteToFile(file, json);
        
        Debug.Log("Game saved with JSON ["+json+"]");
    }

    public void Load()
    {
        string json = ReadFromFile(file);
        if (json != "")
        {
            JsonUtility.FromJsonOverwrite(json, Gm);   
        }
        Debug.Log("Game loaded with JSON ["+json+"]");
    }

    private void WriteToFile(string filename, string json)
    {
        string path = GetFilePath(filename);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
        Debug.Log("Game saved to path ["+path+"]");
    }

    private string ReadFromFile(string filename)
    {
        string path = GetFilePath(filename);
        if (File.Exists(path))
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                return streamReader.ReadToEnd();
            }
        }
        else
        {
            Debug.LogWarning("File [" + path + "] not found!");
            return "";
        }
    }

    private string GetFilePath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
    }
}