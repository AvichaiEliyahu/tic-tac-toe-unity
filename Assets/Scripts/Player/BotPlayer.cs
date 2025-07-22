using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Bot implementation for a player. Plays a random move from the availble cells.
/// </summary>
public class BotPlayer : IPlayer
{
    private readonly List<(int x, int y)> _validMoves = new();
    BotData _botData;

    public BotPlayer(BotData botData)
    {
        _botData = botData;
    }

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

    /// <summary>
    /// Delays the bot play.
    /// </summary>
    /// <returns></returns>
    private async UniTask BotDelay()
    {
        var delay = UnityEngine.Random.Range(_botData.BotMinDelay * 1000, _botData.BotMaxDelay * 1000);
        await UniTask.Delay(delay);
    }

    /// <summary>
    /// Populates the valid moves list according to the available cells.
    /// </summary>
    /// <param name="availableCells">available cells to play</param>
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
