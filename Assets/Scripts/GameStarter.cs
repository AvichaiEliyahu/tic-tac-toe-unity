using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PlayPerfect;
using UnityEngine;

/// <summary>
/// The MonoBehaviour that starts the game and controlls the game flow
/// </summary>
public class GameStarter : MonoBehaviour
{
    [SerializeField] private Transform _boardParent;
    [SerializeField] private GameUI _ui;

    // GamaData is a class contains data for the game to run, including bot times and scoring data.
    // It is currently configured as requested in the instructions but can be modified.
    // We can have a wide selection of GameData objects.
    // For example, game data with faster bot times for QA, or lower score target to check end game scenarios.
    // The game data object is injected into the game manager and from there to the internal components, meanning we can change it from here without interfering with the internal classes.
    [SerializeField] private GameData _gameData;
    [SerializeField] private bool _isUserStart = true;

    private IGameManager _gameManager;
    private CancellationTokenSource _cts;

    private void Start()
    {
        _cts = new CancellationTokenSource();
        _gameManager = new MyGameManager(_boardParent, _cts.Token, _gameData);
        StartGameLoop().Forget();
        _ui.Initialize(() => StartGameLoop().Forget(), GetTotalScore());
    }

    /// <summary>
    /// Start the game loop
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid StartGameLoop()
    {
        try
        {
            _ui.HideEndGameScreen();
            await _gameManager.LoadNewGameAsync(_isUserStart);
            _gameManager.OnGameOver += OnGameEnd;
            while (_gameManager.IsGameInProgress)
            {
                await _gameManager.WaitForPlayerTurn();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>
    /// handle logic for when a game ends
    /// </summary>
    private void OnGameEnd()
    {
        UpdateUI();
        _gameManager.OnGameOver -= OnGameEnd;
    }

    /// <summary>
    /// Updates the UI with current data.
    /// </summary>
    private void UpdateUI()
    {
        _ui.ShowEndGameScreen(score: _gameManager.GetFinalScore());
        _ui.UpdateTotalScoreText(GetTotalScore());
    }

    /// <summary>
    /// Get the total score from the game manager.
    /// </summary>
    /// <returns>The total score</returns>
    private int GetTotalScore()
    {
        return (_gameManager as MyGameManager).TotalScore; // Like the instructions say I did not modify the interface but extended it internally in my implementation.
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
