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
        await InitializeNewBoard().AttachExternalCancellation(_cancellationToken);
        IsGameInProgress = true;
        _playerManager = new PlayerManager(new HumanPlayer(_boardView), new BotPlayer());

        if (_saveManager.HasSavedgameInProgress())
        {
            var data = _saveManager.GetSaveData();
            _boardModel = new BoardModel(data.BoardSize, data.Get2DBoard());
            _scoreManager = new ScoreManager(data.ReactionTimes);
            _playerManager.InitializePlayers(data.PlayerMark == CellState.PlayerX);
            _boardView.DrawBoard(_boardModel.Cells);
        }
        else
        {
            _boardModel = new BoardModel(GameConsts.BOARD_SIZE);
            _scoreManager = new ScoreManager();
            bool humanStarts = isUserFirstTurn ?? UnityEngine.Random.value > 0.5;
            _playerManager.InitializePlayers(humanStarts);
            SaveGame();
        }
    }

    #region Save

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
        TryClearPrevBoard();
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

        TryStartTurnRecording();
        var move = await _playerManager.CurrentPlayer.PlayAsync(availableCells).AttachExternalCancellation(_cancellationToken);
        TryEndTurnRecording();

        UpdateModelBasedOnMove(move);
        SaveGame();
    }

    private void TryStartTurnRecording()
    {
        if(_playerManager.CurrentPlayer is IRecordablePlayer)
        {
            _scoreManager.StartTurn();
        }
    }

    private void TryEndTurnRecording()
    {
        if(_playerManager.CurrentPlayer is IRecordablePlayer)
        {
            _scoreManager.EndTurn();
        }
    }

    #region Play Turn

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
