using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PlayPerfect;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Game manager implamantation for the Tic-Tac-Toe game.
/// Also the main controller which controlls the other sub-managers (score, save, players)
/// </summary>
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
    private GameData _gameData;

    private int _gameScore;
    private CancellationToken _cancellationToken;

    public const string BOARD_VIEW_ADDRESSABLES_KEY = "Assets/Prefabs/Board.prefab";


    public MyGameManager(Transform boardParent, CancellationToken cancellationToken, GameData gameData)
    {
        _boardParent = boardParent;
        _saveManager = new SaveManager();
        _cancellationToken = cancellationToken;
        _gameData = gameData;
    }

    /// <summary>
    /// returns the game final score
    /// </summary>
    /// <returns>Final score</returns>
    public int GetFinalScore()
    {
        return _gameScore;
    }

    /// <summary>
    /// Starts a game, will try to load a save file first.
    /// </summary>
    /// <param name="isUserFirstTurn">Does the user start?</param>
    /// <returns>Awaitable UniTask</returns>
    public async UniTask LoadNewGameAsync(bool? isUserFirstTurn = null)
    {
        await InitializeNewBoard().AttachExternalCancellation(_cancellationToken);
        IsGameInProgress = true;
        _playerManager = new PlayerManager(new HumanPlayer(_boardView), new BotPlayer(_gameData.BotData));

        if (_saveManager.HasSavedgameInProgress())
        {
            var data = _saveManager.GetSaveData();
            _boardModel = new BoardModel(data.BoardSize, data.Get2DBoard());
            _scoreManager = new ScoreManager(_gameData.ScoringSystemData, data.ReactionTimes);
            _playerManager.InitializePlayers(data.PlayerMark == CellState.PlayerX);
            _boardView.DrawBoard(_boardModel.Cells);
        }
        else
        {
            _boardModel = new BoardModel(_gameData.BoardSize);
            _scoreManager = new ScoreManager(_gameData.ScoringSystemData);
            bool humanStarts = isUserFirstTurn ?? UnityEngine.Random.value > 0.5;
            _playerManager.InitializePlayers(humanStarts);
            SaveGame();
        }
    }

    #region Save
    /// <summary>
    /// Saves the game using the save manager
    /// </summary>
    /// <param name="addScore">Optional parameter if a score needs to be added, can be used to add score mid game or at the end game.</param>
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
    /// <summary>
    /// Try to clear the board if exists so that a new fresh board can be loaded
    /// </summary>
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

    /// <summary>
    /// Instantiates a new board
    /// </summary>
    /// <returns>Awaitable UniTask</returns>
    private async UniTask InitializeNewBoard()
    {
        TryClearPrevBoard();
        var boardGo = await Addressables.InstantiateAsync(BOARD_VIEW_ADDRESSABLES_KEY, _boardParent).WithCancellation(_cancellationToken);
        _boardView = boardGo.GetComponent<BoardView>();
        await _boardView.InitializeAsync(_gameData.BoardSize).AttachExternalCancellation(_cancellationToken);
    }
    #endregion

    /// <summary>
    /// Waiting for the current player to play the turn
    /// </summary>
    /// <returns>Awaitable UniTask</returns>
    public async UniTask WaitForPlayerTurn()
    {
        var availableCells = _boardModel.GetAvailableCells();

        TryStartTurnRecording();
        var move = await _playerManager.CurrentPlayer.PlayAsync(availableCells).AttachExternalCancellation(_cancellationToken);
        TryEndTurnRecording();

        UpdateModelBasedOnMove(move);
        SaveGame();
    }

    /// <summary>
    /// Start recording turn time for player if needed
    /// </summary>
    private void TryStartTurnRecording()
    {
        if(_playerManager.CurrentPlayer is IRecordablePlayer)
        {
            _scoreManager.StartTurn();
        }
    }

    /// <summary>
    /// End recording of turn time for player if needed.
    /// </summary>
    private void TryEndTurnRecording()
    {
        if(_playerManager.CurrentPlayer is IRecordablePlayer)
        {
            _scoreManager.EndTurn();
        }
    }

    #region Play Turn
    /// <summary>
    /// Update the model based on the latest move.
    /// </summary>
    /// <param name="move">newly selected cell</param>
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

    /// <summary>
    /// Total score
    /// </summary>
    public int TotalScore => _saveManager?.GetSaveData()?.TotalScore ?? 0;

    #endregion
}
