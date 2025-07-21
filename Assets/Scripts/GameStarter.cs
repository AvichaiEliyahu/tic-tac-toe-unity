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
        _ui.Initialize(() => StartGameLoop().Forget());
    }

    private async UniTaskVoid StartGameLoop()
    {
        _ui.HideReplayView();
        await _gameManager.LoadNewGameAsync(true);
        _gameManager.OnGameOver += OnGameEnd;
        while (_gameManager.IsGameInProgress)
        {
            await _gameManager.WaitForPlayerTurn();
        }
    }

    private void OnGameEnd()
    {
        _ui.ShowReplayView(score: _gameManager.GetFinalScore());
        _gameManager.OnGameOver -= OnGameEnd;        
    }
}
