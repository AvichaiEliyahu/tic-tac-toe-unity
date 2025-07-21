public enum CellState { Empty, PlayerX, PlayerO}
public enum GameResult { None, Win, Lose, Draw}

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

    public CellState GetCellState(int x, int y)
    {
        return Cells[x, y];
    }

    public bool IsCellEmpty(int x, int y)
    {
        return Cells[x, y] == CellState.Empty;
    }

    public bool PlaceMarkOnCell(int x, int y, CellState state)
    {
        if (!(IsCellEmpty(x, y)))
        {
            return false;
        }
        Cells[x, y] = state;
        return true;
    }

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

    private void ClearBoard()
    {
        for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
                Cells[x, y] = CellState.Empty;
    }

    public GameResult CheckGameResult(CellState humanMark, out CellState winner)
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
                    return winner == humanMark ? GameResult.Win : GameResult.Lose;
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
                    return winner == humanMark ? GameResult.Win : GameResult.Lose;
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
                return winner == humanMark ? GameResult.Win : GameResult.Lose;
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
                return winner == humanMark ? GameResult.Win : GameResult.Lose;
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
