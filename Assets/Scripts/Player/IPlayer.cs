using Cysharp.Threading.Tasks;

/// <summary>
/// Interface for the different types of players
/// </summary>
public interface IPlayer
{
    /// <summary>
    /// Plays a move, according to the concrete implementation
    /// </summary>
    /// <param name="availableCells">available cells to play</param>
    /// <returns>awaitable UniTask</returns>
    UniTask<(int x, int y)> PlayAsync(bool[,] availableCells);
}
