using Cysharp.Threading.Tasks;

/// <summary>
/// Possible server implementation for a player. Waits to get the move from the server.
/// </summary>
public class ServerSidePlayer : IPlayer
{
    public UniTask<(int x, int y)> PlayAsync(bool[,] availableCells)
    {
        // For Client-Server game, here we can get the response from the server.
        // return await GetMoveFromServer(availableCells);
        throw new System.NotImplementedException();
    }
}
