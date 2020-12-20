using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string savedGameFilename = "savedGame.tsa";
    private string settingsFilename = "persistentData.tsa";
    
    public void Save(GameManager gm)
    {
        if (!gm.IOEnabled) return;
        
        BinaryFormatter formatter = new BinaryFormatter();
        SaveGame(gm, formatter);
        SaveSettings(gm, formatter);
    }

    private void SaveGame(GameManager gm, BinaryFormatter formatter)
    {
        string path = Application.persistentDataPath + "/" + savedGameFilename;
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData gameData = new GameData(gm);
        formatter.Serialize(stream, gameData);
        stream.Close();
    }

    private void SaveSettings(GameManager gm, BinaryFormatter formatter)
    {
        string path = Application.persistentDataPath + "/" + settingsFilename;
        FileStream stream = new FileStream(path, FileMode.Create);

        PersistentData persistentData = new PersistentData(gm);
        formatter.Serialize(stream, persistentData);
        stream.Close();
    }

    public GameData LoadSavedGame()
    {
        string path = Application.persistentDataPath + "/" + savedGameFilename;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                Debug.LogWarning("Stream was empty; loading saved game failed");
                return null;
            }

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            if (gameData != null && gameData.isPlayerDefeated)
            {
                return null;
            }
            
            return gameData;
        }
        
        Debug.LogWarning("Save file not found in [" + path + "]");
        return null;
    }

    public PersistentData LoadSettings()
    {
        Debug.Log(Application.persistentDataPath + "/");
        
        string path = Application.persistentDataPath + "/" + settingsFilename;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                Debug.LogWarning("Stream was empty; loading settings failed");
                return null;
            }

            PersistentData persistentData = formatter.Deserialize(stream) as PersistentData;
            stream.Close();
            
            return persistentData;
        }
        
        Debug.LogWarning("Settings file not found in [" + path + "]");
        return null;
    }
}