using UnityEngine;

public class PlayerPrefsSaveSystem<T> : ISaveSystem<T>
{

    public void Save(string key, T data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public T Load(string key)
    {
        string json = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<T>(json);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void Delete(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
}
