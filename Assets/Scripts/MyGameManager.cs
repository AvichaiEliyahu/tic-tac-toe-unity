using System;
using Cysharp.Threading.Tasks;
using PlayPerfect;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MyGameManager : IGameManager
{
    public bool IsGameInProgress { get; private set; }

    public event Action OnGameOver;

    private BoardModel _boardModel;
    private BoardView _boardView;
    private IPlayer _playerX;
    private IPlayer _playerO;
    private IPlayer _currentPlayer;
    private CellState _currentMark;
    private Transform _boardParent;

    private GameResult _gameResult;
    private ScoreSystem _scoreSystem;


    public MyGameManager(Transform boardParent)
    {
        _boardParent = boardParent;
    }

    public int GetFinalScore()
    {
        return _scoreSystem.CalcFinalScore(_gameResult);
    }

    public async UniTask LoadNewGameAsync(bool? isUserFirstTurn = null)
    {
        IsGameInProgress = true;
        _boardModel = new BoardModel(GameConsts.BOARD_SIZE);
        _scoreSystem = new ScoreSystem();

        TryClearPrevBoard();

        await InitializeNewBoard();

        InitializePlayers(isUserFirstTurn);
    }

    #region New Game Initialization
    private void TryClearPrevBoard()
    {
        if (_boardView == null)
        {
            return;
        }
        _boardView.DestroyBoard();
        Addressables.ReleaseInstance(_boardView.gameObject);
        GameObject.Destroy(_boardView.gameObject);
    }

    private async UniTask InitializeNewBoard()
    {
        var boardGo = await Addressables.InstantiateAsync(GameConsts.BOARD_VIEW_ADDRESSABLES_KEY, _boardParent);
        _boardView = boardGo.GetComponent<BoardView>();
        await _boardView.InitializeAsync();
    }

    private void InitializePlayers(bool? isUserFirstTurn)
    {
        _playerX = new HumanPlayer();
        _playerO = new BotPlayer();

        bool humanStarts = isUserFirstTurn ?? UnityEngine.Random.value > 0.5;
        _currentPlayer = humanStarts ? _playerX : _playerO;
        _currentMark = _currentPlayer == _playerX ? CellState.PlayerX : CellState.PlayerO;
    }

    #endregion

    public async UniTask WaitForPlayerTurn()
    {
        var availableCells = _boardModel.GetAvailableCells();

        var move = await WaitForMove(availableCells);

        UpdateModelBasedOnMove(move);
    }

    #region Play Turn
    private async UniTask<(int x, int y)> WaitForMove(bool[,] availableCells)
    {
        if (_currentPlayer is BotPlayer)
        {
            return await _currentPlayer.PlayAsync(availableCells);
        }
        else
        {
            _scoreSystem.StartTurn();
            var move = await _boardView.WaitForPress(availableCells);
            _scoreSystem.EndTurn();
            return move;
        }
    }

    private void UpdateModelBasedOnMove((int x, int y) move)
    {
        if (_boardModel.PlaceMarkOnCell(move.x, move.y, _currentMark))
        {
            _boardView.DrawBoard(_boardModel.Cells);
            var result = _boardModel.CheckGameResult(CellState.PlayerX, out var winner);
            if (result != GameResult.None)
            {
                _gameResult = result;
                IsGameInProgress = false;
                OnGameOver?.Invoke();
                return;
            }

            (_currentPlayer, _currentMark) =
                _currentPlayer == _playerX ?
                (_playerO, CellState.PlayerO) :
                (_playerX, CellState.PlayerX);
        }
    }
    #endregion
}
