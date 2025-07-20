using Cysharp.Threading.Tasks;

public interface IPlayer
{
    UniTask<(int x, int y)> PlayAsync(bool[,] availableCells);
}
