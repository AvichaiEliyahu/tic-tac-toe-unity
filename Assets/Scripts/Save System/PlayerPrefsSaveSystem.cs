using UnityEngine;

/// <summary>
/// Save system implementation for saving the game in player prefs.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PlayerPrefsSaveSystem<T> : ISaveSystem<T>
{

    /// <summary>
    /// Save to player prefs.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    public void Save(string key, T data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load from player prefs.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Load(string key)
    {
        string json = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// Does this key exists in the player prefs?
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True if the key exists</returns>
    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// Delete from player prefs.
    /// </summary>
    /// <param name="key">The key at which we want to delete.</param>
    public void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
}
