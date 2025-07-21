using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int BoardSize;
    public CellState[] FlatCells;
    public bool IsGameInProgress;

    public CellState CurrentMark;
    public CellState PlayerMark;
    public List<float> ReactionTimes;

    public int TotalScore;

    public GameSaveData() { }

    public GameSaveData(BoardModel board, bool isGameInProgress, CellState currentMark, CellState playerMark, List<float> reactionTimes)
    {
        BoardSize = board.Size;
        FlatCells = Flatten(board.Cells);
        IsGameInProgress = isGameInProgress;
        CurrentMark = currentMark;
        PlayerMark = playerMark;
        ReactionTimes = new List<float>(reactionTimes);
    }

    public CellState[,] Get2DBoard()
    {
        return Unflatten(FlatCells, BoardSize);
    }

    private static CellState[] Flatten(CellState[,] cells)
    {
        int size = cells.GetLength(0);
        var flat = new CellState[size * size];
        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                flat[i * size + j] = cells[i, j];
        return flat;
    }

    private static CellState[,] Unflatten(CellState[] flat, int size)
    {
        var cells = new CellState[size, size];
        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                cells[i, j] = flat[i * size + j];
        return cells;
    }
}
