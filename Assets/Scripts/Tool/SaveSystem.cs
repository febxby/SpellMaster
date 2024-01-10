using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public static class SaveSystem
{
    public static void SaveByJson(string saveFileName, object data)
    {
        string json = JsonUtility.ToJson(data, true);
        Debug.Log(json);
        //Application.persistentDataPath:Unity提供的一个存储永久数据的路径：不同平台上路径不同。
        //Path.Combine：合并传入的路径。
        var path = Path.Combine(Application.dataPath, saveFileName + ".json");
        //File.WriteAllText方法用于写入字符串类型的数据，而Json是字符串类型。
        File.WriteAllText(path, json);
    }
    public static T LoadFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.dataPath, saveFileName + ".json");
        if (!File.Exists(path))
            return default(T);
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<T>(json);
        return data;

    }
    public static T LoadFromJson<T>(string saveFileName, T defaultValue)
    {
        var path = Path.Combine(Application.dataPath, saveFileName + ".json");
        if (!File.Exists(path))
            return defaultValue;
        var json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, defaultValue);
        return defaultValue;

    }
    public static List<T> LoadListFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.dataPath, saveFileName + ".json");
        if (!File.Exists(path))
            return default(List<T>);
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SerializationList<T>>(json).ToList();
        return data;

    }
    public static void DeleteSaveFile(string saveFileName)
    {
        var path = Path.Combine(Application.dataPath, saveFileName + ".json");
        File.Delete(path);
    }
}
