using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveAndLoadSystem
{
    private static string filename = "gameData.cid";
    
    public static void Save(GameManager gm)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + filename;
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData gameData = new GameData(gm);
        formatter.Serialize(stream, gameData);
        stream.Close();
    }
    
    public static GameData Load()
    {
        string path = Application.persistentDataPath + "/" + filename;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return gameData;
        }
        
        Debug.LogError("Save file not found in [" + path + "]");
        return null;
    }

    public static bool SavedGameExists()
    {
        return File.Exists(Application.persistentDataPath + "/" + filename);
    }
}