using System.IO;
using UnityEngine;
public static class SaveLoadSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/SaveData/";
    private static readonly string SAVE_EXTENSION = "txt";
    private static readonly string REGEX_ONLY_NUMBERS = "^[0-9]*$";

    private static bool isInit = false;

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }
    }
    // save
    public static void Save(string fileName, string saveString, bool overwrite)
    {
        Init();
        string saveFileName = fileName + "_0";
        if (!overwrite)
        {
            int saveNumber = 0;
            while (File.Exists(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION))
            {
                saveNumber++;
                saveFileName = fileName + "_" + saveNumber;
            }
        }
        File.WriteAllText(SAVE_FOLDER + saveFileName + "." + SAVE_EXTENSION, saveString);
    }
    public static void SaveObject(object saveObject)
    {
        SaveObject("save", saveObject, false);
    }
    public static void SaveObject(string fileName, object saveObject, bool overwrite)
    {
        Init();
        string json = JsonUtility.ToJson(saveObject);
        Save(fileName, json, overwrite);
    }
    // load - returns null if empty
    public static string Load(string fileName)
    {
        Init();
        if (File.Exists(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + fileName + "." + SAVE_EXTENSION);
            return saveString;
        }
        else
        {
            return null;
        }
    }
    public static string LoadMostRecent()
    {
        return LoadMostRecent(null);
    }
    public static string LoadMostRecent(string fileName)
    {
        Init();
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);

        FileInfo[] saveFiles;
        if (fileName == null)
        {
            saveFiles = directoryInfo.GetFiles(fileName + "_" + "*" + "." + SAVE_EXTENSION);
        }
        else
        {
            saveFiles = directoryInfo.GetFiles(fileName + "_" + REGEX_ONLY_NUMBERS + "." + SAVE_EXTENSION);
        }

        FileInfo mostRecentFile = null;
        foreach (FileInfo fileInfo in saveFiles)
        {
            if (mostRecentFile == null)
            {
                mostRecentFile = fileInfo;
            }
            else
            {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime)
                {
                    mostRecentFile = fileInfo;
                }
            }
        }

        if (mostRecentFile != null)
        {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        }
        else
        {
            return null;
        }
    }
    public static T_SaveObject LoadMostRecentObject<T_SaveObject>()
    {
        string saveString = LoadMostRecent();
        if (saveString != null)
        {
            T_SaveObject saveObject = JsonUtility.FromJson<T_SaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(T_SaveObject);
        }
    }
    public static T_SaveObject LoadMostRecentObject<T_SaveObject>(string fileName)
    {
        Init();
        string saveString = LoadMostRecent(fileName);
        if (saveString != null)
        {
            T_SaveObject saveObject = JsonUtility.FromJson<T_SaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(T_SaveObject);
        }
    }
    public static T_SaveObject LoadObject<T_SaveObject>(string fileName)
    {
        Init();
        string saveString = Load(fileName);
        if (saveString != null)
        {
            T_SaveObject saveObject = JsonUtility.FromJson<T_SaveObject>(saveString);
            return saveObject;
        }
        else
        {
            return default(T_SaveObject);
        }
    }
}