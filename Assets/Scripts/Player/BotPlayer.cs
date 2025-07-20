using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class BotPlayer : IPlayer
{
    private readonly List<(int x, int y)> _validMoves = new();
    public async UniTask<(int x, int y)> PlayAsync(bool[,] availableCells)
    {
        await BotDelay();
        PopulateValidMoves(availableCells);

        if(_validMoves.Count == 0)
        {
            throw new InvalidOperationException("No valid moves left for the bot.");
        }

        return _validMoves[UnityEngine.Random.Range(0, _validMoves.Count)];
    }

    private async UniTask BotDelay()
    {
        var delay = UnityEngine.Random.Range(GameConsts.BOT_DELAY_MIN * 1000, GameConsts.BOT_DELAY_MAX * 1000);
        await UniTask.Delay(delay);
    }

    private void PopulateValidMoves(bool[,] availableCells)
    {
        _validMoves.Clear();
        var width = availableCells.GetLength(0);
        var height = availableCells.GetLength(1);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (availableCells[x, y])
                {
                    _validMoves.Add((x, y));
                }
            }
        }
    }
}
