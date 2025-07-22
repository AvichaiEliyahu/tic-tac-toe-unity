using System;
using System.Collections.Generic;

/// <summary>
/// Class that contains the data that we want to save.
/// Since Serialization does not work for 2D arrays (matrix), there is a way to "flatten" and "unflatten" the matrix.
/// </summary>
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

    /// <summary>
    /// Return the board in its original 2D array representation
    /// </summary>
    /// <returns>The matrix of cells</returns>
    public CellState[,] Get2DBoard()
    {
        return Unflatten(FlatCells, BoardSize);
    }

    /// <summary>
    /// Turns the 2D array of cells into 1D regular array, so it can be serialized and saved.
    /// </summary>
    /// <param name="cells">Matrix of cells</param>
    /// <returns>Flatten array of cells</returns>
    private static CellState[] Flatten(CellState[,] cells)
    {
        int size = cells.GetLength(0);
        var flat = new CellState[size * size];
        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                flat[i * size + j] = cells[i, j];
        return flat;
    }

    /// <summary>
    /// Turns the "flat" array into a matrix
    /// </summary>
    /// <param name="flat">The flat array</param>
    /// <param name="size">The size of the matrix</param>
    /// <returns></returns>
    private static CellState[,] Unflatten(CellState[] flat, int size)
    {
        var cells = new CellState[size, size];
        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                cells[i, j] = flat[i * size + j];
        return cells;
    }
}
