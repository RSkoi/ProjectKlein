using System.IO;
using System.Text;
using System;
using UnityEngine;

// https://stackoverflow.com/questions/40965645/what-is-the-best-way-to-save-game-state/40966346#40966346
public class DataSaver
{
    private static readonly string PATH = "CrossSceneData";
    private static readonly string FILE_ENDING = ".json";

    public static void SaveData<T>(T dataToSave, string dataFileName)
    {
        SaveData(dataToSave, dataFileName, PATH, FILE_ENDING);
    }

    //Save Data
    public static void SaveData<T>(T dataToSave, string dataFileName, string path, string fileEnding)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, path);
        tempPath = Path.Combine(tempPath, dataFileName + fileEnding);

        //Convert To Json then to bytes
        string jsonData = JsonUtility.ToJson(dataToSave, true);
        byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        //Debug.Log(path);

        try
        {
            File.WriteAllBytes(tempPath, jsonByte);
            Debug.Log("Saved Data to: " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public static T LoadData<T>(string dataFileName)
    {
        return LoadData<T>(dataFileName, PATH, FILE_ENDING);
    }

    //Load Data
    public static T LoadData<T>(string dataFileName, string path, string fileEnding)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, path);
        tempPath = Path.Combine(tempPath, dataFileName + fileEnding);

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Debug.LogWarning("Directory does not exist");
            return default(T);
        }

        if (!File.Exists(tempPath))
        {
            Debug.Log("File does not exist");
            return default(T);
        }

        //Load saved Json
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(tempPath);
            Debug.Log("Loaded Data from: " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Load Data from: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        //Convert to json string
        string jsonData = Encoding.ASCII.GetString(jsonByte);

        //Convert to Object
        object resultValue = JsonUtility.FromJson<T>(jsonData);
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }

    public static bool DeleteData(string dataFileName)
    {
        return DeleteData(dataFileName, PATH, FILE_ENDING);
    }

    public static bool DeleteData(string dataFileName, string path, string fileEnding)
    {
        bool success = false;

        //Load Data
        string tempPath = Path.Combine(Application.persistentDataPath, path);
        tempPath = Path.Combine(tempPath, dataFileName + fileEnding);

        //Exit if Directory or File does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Debug.LogWarning("Directory does not exist");
            return false;
        }

        if (!File.Exists(tempPath))
        {
            Debug.Log("File does not exist");
            return false;
        }

        try
        {
            File.Delete(tempPath);
            Debug.Log("Data deleted from: " + tempPath.Replace("/", "\\"));
            success = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Delete Data: " + e.Message);
        }

        return success;
    }
}
