using Cysharp.Threading.Tasks;
using PlayPerfect;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private Transform _boardParent;
    [SerializeField] private GameUI _ui;

    private IGameManager _gameManager;

    private void Start()
    {
        _gameManager = new MyGameManager(_boardParent);
        StartGameLoop().Forget();
        _ui.Initialize(() => StartGameLoop().Forget(), GetTotalScore());
    }

    private async UniTaskVoid StartGameLoop()
    {
        _ui.HideEndGameScreen();
        await _gameManager.LoadNewGameAsync(true);
        _gameManager.OnGameOver += OnGameEnd;
        while (_gameManager.IsGameInProgress)
        {
            await _gameManager.WaitForPlayerTurn();
        }
    }

    private void OnGameEnd()
    {
        UpdateUI();
        _gameManager.OnGameOver -= OnGameEnd;        
    }

    private void UpdateUI()
    {
        _ui.ShowEndGameScreen(score: _gameManager.GetFinalScore());
        _ui.UpdateTotalScoreText(GetTotalScore());
    }

    private int GetTotalScore()
    {
        return (_gameManager as MyGameManager).TotalScore; // I don't like it but since IGameManager interface can not be changed I choose to do this.
    }
}
