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

    public bool HasSavedgameInProgress()
    {
        return _cachedData != null && _cachedData.IsGameInProgress;
    }

    public GameSaveData GetSaveData()
    {
        return _cachedData;
    }

    public void Save(GameSaveData data)
    {
        _cachedData = data;
        _saveSystem.Save(SAVE_KEY, data);
    }

    public void Load()
    {
        if (_saveSystem.HasKey(SAVE_KEY))
        {
            _cachedData = _saveSystem.Load(SAVE_KEY);
        }
    }
}
