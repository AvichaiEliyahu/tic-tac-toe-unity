using System;
using System.Threading;
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
    private Transform _boardParent;

    private ScoreManager _scoreManager;
    private PlayerManager _playerManager;
    private SaveManager _saveManager;

    private int _gameScore;
    private CancellationToken _cancellationToken;

    public MyGameManager(Transform boardParent, CancellationToken cancellationToken)
    {
        _boardParent = boardParent;
        _saveManager = new SaveManager();
        _cancellationToken = cancellationToken;
    }

    public int GetFinalScore()
    {
        return _gameScore;
    }

    public async UniTask LoadNewGameAsync(bool? isUserFirstTurn = null)
    {
        if (_saveManager.HasSavedgameInProgress())
        {
            await LoadFromSave(_saveManager.GetSaveData()).AttachExternalCancellation(_cancellationToken);
            return;
        }
        await StartNewGame(isUserFirstTurn).AttachExternalCancellation(_cancellationToken);
    }

    private async UniTask StartNewGame(bool? isUserFirstTurn)
    {
        IsGameInProgress = true;
        _boardModel = new BoardModel(GameConsts.BOARD_SIZE);
        _playerManager = new PlayerManager();
        _scoreManager = new ScoreManager();

        TryClearPrevBoard();

        await InitializeNewBoard().AttachExternalCancellation(_cancellationToken);

        InitializePlayers(isUserFirstTurn);
        SaveGame();
    }

    #region Save and Load
    private async UniTask LoadFromSave(GameSaveData data)
    {
        _boardModel = new BoardModel(data.BoardSize, data.Get2DBoard());
        _scoreManager = new ScoreManager(data.ReactionTimes);
        _playerManager = new PlayerManager();
        _playerManager.InitializePlayers(data.PlayerMark == CellState.PlayerX);
        TryClearPrevBoard();
        await InitializeNewBoard().AttachExternalCancellation(_cancellationToken);
        _boardView.DrawBoard(_boardModel.Cells);
        IsGameInProgress = true;
    }

    private void SaveGame(int addScore = 0)
    {
        var currentSave = _saveManager.GetSaveData() ?? new GameSaveData();
        var newData = new GameSaveData(_boardModel, IsGameInProgress, _playerManager.CurrentMark, CellState.PlayerX, _scoreManager.GetCurrentReactionTimes())
        {
            TotalScore = currentSave.TotalScore + addScore
        };
        _saveManager.Save(newData);
    }
    #endregion
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
        var boardGo = await Addressables.InstantiateAsync(GameConsts.BOARD_VIEW_ADDRESSABLES_KEY, _boardParent).WithCancellation(_cancellationToken);
        _boardView = boardGo.GetComponent<BoardView>();
        await _boardView.InitializeAsync().AttachExternalCancellation(_cancellationToken);
    }

    private void InitializePlayers(bool? isUserFirstTurn)
    {
        bool humanStarts = isUserFirstTurn ?? UnityEngine.Random.value > 0.5;
        _playerManager.InitializePlayers(humanStarts);
    }

    #endregion

    public async UniTask WaitForPlayerTurn()
    {
        var availableCells = _boardModel.GetAvailableCells();

        var move = await WaitForMove(availableCells).AttachExternalCancellation(_cancellationToken);
        UpdateModelBasedOnMove(move);
        SaveGame();
    }

    #region Play Turn
    private async UniTask<(int x, int y)> WaitForMove(bool[,] availableCells)
    {
        if (_playerManager.CurrentPlayer is BotPlayer)
        {
            return await _playerManager.CurrentPlayer.PlayAsync(availableCells).AttachExternalCancellation(_cancellationToken);
        }
        else
        {
            _scoreManager.StartTurn();
            var move = await _boardView.WaitForPress(availableCells).AttachExternalCancellation(_cancellationToken);
            _scoreManager.EndTurn();
            return move;
        }
    }

    private void UpdateModelBasedOnMove((int x, int y) move)
    {
        if (_boardModel.PlaceMarkOnCell(move.x, move.y, _playerManager.CurrentMark))
        {
            _boardView.DrawBoard(_boardModel.Cells);
            var result = _boardModel.CheckGameResult(CellState.PlayerX, out var winner);
            if (result != GameResult.None)
            {
                IsGameInProgress = false;
                _gameScore = _scoreManager.CalcFinalScore(result);
                
                SaveGame(_gameScore);
                OnGameOver?.Invoke();
                return;
            }

            _playerManager.SwitchTurn();
        }
    }

    public int TotalScore => _saveManager?.GetSaveData()?.TotalScore ?? 0;

    #endregion
}
