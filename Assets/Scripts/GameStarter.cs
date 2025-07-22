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

    private IGameManager _gameManager;
    private CancellationTokenSource _cts;

    private void Start()
    {
        _cts = new CancellationTokenSource();
        _gameManager = new MyGameManager(_boardParent, _cts.Token);
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
            await _gameManager.LoadNewGameAsync(true);
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
