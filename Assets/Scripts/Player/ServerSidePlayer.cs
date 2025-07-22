using Cysharp.Threading.Tasks;

public class ServerSidePlayer : IPlayer
{
    public UniTask<(int x, int y)> PlayAsync(bool[,] availableCells)
    {
        // For Client-Server game, here we can get the response from the server.
        // return await GetMoveFromServer(availableCells);
        throw new System.NotImplementedException();
    }
}
