using UnityEngine;

public class PlayerManager
{
    private IPlayer _playerX;
    private IPlayer _playerO;
    private IPlayer _currentPlayer;

    public IPlayer CurrentPlayer => _currentPlayer;
    public CellState CurrentMark => _currentPlayer == _playerX ? CellState.PlayerX : CellState.PlayerO;

    public void InitializePlayers(bool playerStarts)
    {
        _playerX = new HumanPlayer();
        _playerO = new BotPlayer();
        _currentPlayer = playerStarts ? _playerX : _playerO;
    }

    public void SwitchTurn()
    {
        _currentPlayer = _currentPlayer == _playerX ? _playerO : _playerX;
    }
}
