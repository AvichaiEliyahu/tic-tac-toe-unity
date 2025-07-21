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
    private Transform _boardParent;

    private ScoreSystem _scoreSystem;
    private PlayerManager _playerManager;
    private SaveManager _saveManager;

    private int _gameScore;

    public MyGameManager(Transform boardParent)
    {
        _boardParent = boardParent;
        _saveManager = new SaveManager();
    }

    public int GetFinalScore()
    {
        return _gameScore;
    }

    public async UniTask LoadNewGameAsync(bool? isUserFirstTurn = null)
    {
        if (_saveManager.HasSavedgameInProgress())
        {
            await LoadFromSave(_saveManager.GetSaveData());
            return;
        }
        await StartNewGame(isUserFirstTurn);
    }

    private async UniTask StartNewGame(bool? isUserFirstTurn)
    {
        IsGameInProgress = true;
        _boardModel = new BoardModel(GameConsts.BOARD_SIZE);
        _playerManager = new PlayerManager();
        _scoreSystem = new ScoreSystem();

        TryClearPrevBoard();

        await InitializeNewBoard();

        InitializePlayers(isUserFirstTurn);
        SaveGame();
    }

    private async UniTask LoadFromSave(GameSaveData data)
    {
        _boardModel = new BoardModel(data.BoardSize, data.Get2DBoard());
        _scoreSystem = new ScoreSystem(data.ReactionTimes);
        _playerManager = new PlayerManager();
        _playerManager.InitializePlayers(data.PlayerMark == CellState.PlayerX);
        TryClearPrevBoard();
        await InitializeNewBoard();
        _boardView.DrawBoard(_boardModel.Cells);
        IsGameInProgress = true;
    }

    private void SaveGame(int addScore = 0)
    {
        var currentSave = _saveManager.GetSaveData() ?? new GameSaveData();
        var newData = new GameSaveData(_boardModel, IsGameInProgress, _playerManager.CurrentMark, CellState.PlayerX, _scoreSystem.GetCurrentReactionTimes())
        {
            TotalScore = currentSave.TotalScore + addScore
        };
        _saveManager.Save(newData);
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
        bool humanStarts = isUserFirstTurn ?? UnityEngine.Random.value > 0.5;
        _playerManager.InitializePlayers(humanStarts);
    }

    #endregion

    public async UniTask WaitForPlayerTurn()
    {
        var availableCells = _boardModel.GetAvailableCells();

        var move = await WaitForMove(availableCells);
        UpdateModelBasedOnMove(move);
        SaveGame();
    }

    #region Play Turn
    private async UniTask<(int x, int y)> WaitForMove(bool[,] availableCells)
    {
        if (_playerManager.CurrentPlayer is BotPlayer)
        {
            return await _playerManager.CurrentPlayer.PlayAsync(availableCells);
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
        if (_boardModel.PlaceMarkOnCell(move.x, move.y, _playerManager.CurrentMark))
        {
            _boardView.DrawBoard(_boardModel.Cells);
            var result = _boardModel.CheckGameResult(CellState.PlayerX, out var winner);
            if (result != GameResult.None)
            {
                IsGameInProgress = false;
                _gameScore = _scoreSystem.CalcFinalScore(result);
                
                SaveGame(_gameScore);
                OnGameOver?.Invoke();
                return;
            }

            _playerManager.SwitchTurn();
        }
    }

    public int TotalScore => _saveManager.GetSaveData().TotalScore;

    #endregion
}
