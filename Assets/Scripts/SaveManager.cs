/// <summary>
/// Class for handling saving and loading, using the implemented save systems.
/// </summary>
public class SaveManager
{
    private ISaveSystem<GameSaveData> _saveSystem;
    private const string SAVE_KEY = "CurrentGameSave";
    private GameSaveData _cachedData;

    public SaveManager()
    {
#if UNITY_EDITOR
        _saveSystem = new FileSaveSystem<GameSaveData>();
#else
        _saveSystem = new PlayerPrefsSaveSystem<GameSaveData>();
#endif
        Load();
    }

    /// <summary>
    /// Checks if a game is already in progress.
    /// </summary>
    /// <returns>True if a game is in progress</returns>
    public bool HasSavedgameInProgress()
    {
        return _cachedData != null && _cachedData.IsGameInProgress;
    }

    /// <summary>
    /// Get the current saved data.
    /// </summary>
    /// <returns>The save data</returns>
    public GameSaveData GetSaveData()
    {
        return _cachedData;
    }

    /// <summary>
    /// Saves the data
    /// </summary>
    /// <param name="data">Data to save</param>
    public void Save(GameSaveData data)
    {
        _cachedData = data;
        _saveSystem.Save(SAVE_KEY, data);
    }

    /// <summary>
    /// Loads the data
    /// </summary>
    public void Load()
    {
        if (_saveSystem.HasKey(SAVE_KEY))
        {
            _cachedData = _saveSystem.Load(SAVE_KEY);
        }
    }
}
