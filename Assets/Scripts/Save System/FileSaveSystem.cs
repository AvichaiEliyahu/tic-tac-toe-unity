using System.IO;
using UnityEngine;

public class FileSaveSystem<T> : ISaveSystem<T>
{
    private const string FolderName = "Save Files";

    public void Save(string key, T data)
    {
        string path = GetPathForKey(key);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // apply changes
#endif
    }

    public T Load(string key)
    {
        var path = GetPathForKey(key);
        if (!File.Exists(path))
        {
            Debug.LogError("No save file found at " + path);
            return default;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    public bool HasKey(string key)
    {
        string path = GetPathForKey(key);
        return File.Exists(path);
    }

    public void Delete(string key)
    {
        string path = GetPathForKey(key);
        if (File.Exists(path))
        {
            File.Delete(path);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(); // apply changes
#endif
        }
    }

    private string GetPathForKey(string key)
    {
        return Path.Combine(Application.dataPath, FolderName, $"{key}.json");
    }

}
