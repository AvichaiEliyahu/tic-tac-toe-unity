public enum CellState { Empty, PlayerX, PlayerO}
public enum GameResult { None, Win, Lose, Draw}

/// <summary>
/// Board model
/// </summary>
public class BoardModel
{
    private readonly int _size;

    public CellState[,] Cells { get; private set; }

    public int Size => _size;

    public BoardModel(int size)
    {
        _size = size;
        Cells = new CellState[size, size];
        ClearBoard();
    }

    public BoardModel(int size, CellState[,] cellStates) : this(size)
    {
        Cells = cellStates;
    }

    /// <summary>
    /// Checking whether or not a cell is empty.
    /// </summary>
    /// <param name="x">X coordinate of the cell</param>
    /// <param name="y">Y coordinate of the cell</param>
    /// <returns>true if the cell is empty, flase otherwise</returns>
    public bool IsCellEmpty(int x, int y)
    {
        return Cells[x, y] == CellState.Empty;
    }

    /// <summary>
    /// Places a give mark on a given cell
    /// </summary>
    /// <param name="x">X coordinate of the cell</param>
    /// <param name="y">Y coordinate of the cell</param>
    /// <param name="state">New state for the cell</param>
    /// <returns>True if placement succeeded, false otherwise</returns>
    public bool PlaceMarkOnCell(int x, int y, CellState state)
    {
        if (!(IsCellEmpty(x, y)))
        {
            return false;
        }
        Cells[x, y] = state;
        return true;
    }

    /// <summary>
    /// Get all available cells
    /// </summary>
    /// <returns>Matrix of booleans that indicate availability of each cell</returns>
    public bool[,] GetAvailableCells()
    {
        var available = new bool[_size, _size];
        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                available[x, y] = Cells[x, y] == CellState.Empty;
            }
        }
        return available;
    }

    /// <summary>
    /// Clears the board model
    /// </summary>
    private void ClearBoard()
    {
        for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
                Cells[x, y] = CellState.Empty;
    }

    /// <summary>
    /// Check the game result according to the current board state
    /// </summary>
    /// <param name="check">mark to check if won</param>
    /// <param name="winner">winning mark, if any</param>
    /// <returns>The result of the game according to the check mark and the winning mark</returns>
    public GameResult CheckGameResult(CellState check, out CellState winner)
    {
        winner = CellState.Empty;

        // Check rows and columns
        for (int i = 0; i < _size; i++)
        {
            // Check row
            if (Cells[i, 0] != CellState.Empty)
            {
                bool rowWin = true;
                for (int j = 1; j < _size; j++)
                {
                    if (Cells[i, j] != Cells[i, 0])
                    {
                        rowWin = false;
                        break;
                    }
                }
                if (rowWin)
                {
                    winner = Cells[i, 0];
                    return winner == check ? GameResult.Win : GameResult.Lose;
                }
            }

            // Check column
            if (Cells[0, i] != CellState.Empty)
            {
                bool colWin = true;
                for (int j = 1; j < _size; j++)
                {
                    if (Cells[j, i] != Cells[0, i])
                    {
                        colWin = false;
                        break;
                    }
                }
                if (colWin)
                {
                    winner = Cells[0, i];
                    return winner == check ? GameResult.Win : GameResult.Lose;
                }
            }
        }

        // Check main diagonal
        if (Cells[0, 0] != CellState.Empty)
        {
            bool diagWin = true;
            for (int i = 1; i < _size; i++)
            {
                if (Cells[i, i] != Cells[0, 0])
                {
                    diagWin = false;
                    break;
                }
            }
            if (diagWin)
            {
                winner = Cells[0, 0];
                return winner == check ? GameResult.Win : GameResult.Lose;
            }
        }

        // Check anti-diagonal
        if (Cells[0, _size - 1] != CellState.Empty)
        {
            bool antiDiagWin = true;
            for (int i = 1; i < _size; i++)
            {
                if (Cells[i, _size - 1 - i] != Cells[0, _size - 1])
                {
                    antiDiagWin = false;
                    break;
                }
            }
            if (antiDiagWin)
            {
                winner = Cells[0, _size - 1];
                return winner == check ? GameResult.Win : GameResult.Lose;
            }
        }

        // Check for draw (no empty cells left)
        bool anyEmpty = false;
        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                if (Cells[x, y] == CellState.Empty)
                {
                    anyEmpty = true;
                    break;
                }
            }
            if (anyEmpty) break;
        }

        if (!anyEmpty)
            return GameResult.Draw;

        return GameResult.None;
    }

}
