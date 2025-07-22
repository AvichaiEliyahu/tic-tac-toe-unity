/// <summary>
/// Interface for saving the game
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISaveSystem<T>
{
    void Save(string key, T data);
    T Load(string key);
    bool HasKey(string key);
    void Delete(string key);
}
