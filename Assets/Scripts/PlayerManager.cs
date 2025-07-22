/// <summary>
/// Class for managing the players in game
/// </summary>
public class PlayerManager
{
    private IPlayer _playerX;
    private IPlayer _playerO;
    private IPlayer _currentPlayer;

    public IPlayer CurrentPlayer => _currentPlayer;
    public CellState CurrentMark => _currentPlayer == _playerX ? CellState.PlayerX : CellState.PlayerO;

    public PlayerManager(IPlayer playerX, IPlayer playerO)
    {
        _playerX = playerX;
        _playerO = playerO;
    }

    /// <summary>
    /// Initializes the players
    /// </summary>
    /// <param name="playerStarts">Does the user start?</param>
    public void InitializePlayers(bool playerStarts)
    {
        _currentPlayer = playerStarts ? _playerX : _playerO;
    }

    /// <summary>
    /// Moves the turn to the mext player
    /// </summary>
    public void SwitchTurn()
    {
        _currentPlayer = _currentPlayer == _playerX ? _playerO : _playerX;
    }
}
