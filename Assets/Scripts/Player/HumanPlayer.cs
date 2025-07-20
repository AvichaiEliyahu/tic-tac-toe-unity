using Cysharp.Threading.Tasks;

public class HumanPlayer : IPlayer, IClickablePlayer
{
    private UniTaskCompletionSource<(int x, int y)> _tcs;

    public UniTask<(int x, int y)> PlayAsync(bool[,] availableCells)
    {
        _tcs = new();
        return _tcs.Task;
    }

    public void OnCellClicked(int x, int y)
    {
        if(_tcs != null)
        {
            _tcs.TrySetResult((x, y));
        }
    }
}
