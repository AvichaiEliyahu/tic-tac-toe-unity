using Cysharp.Threading.Tasks;

public class HumanPlayer : IPlayer, IRecordablePlayer
{
    private BoardView _boardView;

    public HumanPlayer(BoardView boardView)
    {
        _boardView = boardView;
    }

    public async UniTask<(int x, int y)> PlayAsync(bool[,] availableCells)
    {
        return await _boardView.WaitForPress(availableCells);
    }
}
