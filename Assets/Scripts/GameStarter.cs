using Cysharp.Threading.Tasks;
using PlayPerfect;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private Transform _boardParent;

    private IGameManager _gameManager;

    private void Start()
    {
        StartGameLoop().Forget();
    }

    private async UniTaskVoid StartGameLoop()
    {
        _gameManager = new MyGameManager(_boardParent);
        await _gameManager.LoadNewGameAsync(true);
        _gameManager.OnGameOver += OnGameEnd;
        while (_gameManager.IsGameInProgress)
        {
            await _gameManager.WaitForPlayerTurn();
        }
    }

    private void OnGameEnd()
    {
        Debug.Log("Game Over!");
    }
}
