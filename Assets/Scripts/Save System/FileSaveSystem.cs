using System.IO;
using UnityEngine;

/// <summary>
/// Save system implementation for saving the game in a file.
/// </summary>
/// <typeparam name="T"></typeparam>
public class FileSaveSystem<T> : ISaveSystem<T>
{
    private const string FolderName = "Save Files";

    /// <summary>
    /// Saves the data to a file
    /// </summary>
    /// <param name="key">The file name, also used as the key for saving</param>
    /// <param name="data">Data to save</param>
    public void Save(string key, T data)
    {
        string path = GetPathForKey(key);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh(); // apply changes
#endif
    }

    /// <summary>
    /// Loading the save from the file
    /// </summary>
    /// <param name="key">The file name from which we want to load</param>
    /// <returns>The loaded data</returns>
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

    /// <summary>
    /// Check whether the file exists.
    /// </summary>
    /// <param name="key">File name</param>
    /// <returns>ture is key exists</returns>
    public bool HasKey(string key)
    {
        string path = GetPathForKey(key);
        return File.Exists(path);
    }

    /// <summary>
    /// Deletes the save file.
    /// </summary>
    /// <param name="key">File to delete</param>
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

    /// <summary>
    /// Get the proper path according to the file name. All files are in the same folder.
    /// </summary>
    /// <param name="key">File name</param>
    /// <returns>The full path, based on the given file name</returns>
    private string GetPathForKey(string key)
    {
        return Path.Combine(Application.dataPath, FolderName, $"{key}.json");
    }

}
