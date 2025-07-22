using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles game UI
/// </summary>
public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private TextMeshProUGUI _gameScoreText;
    [SerializeField] private TextMeshProUGUI _totalScoreText;
    [SerializeField] private GameObject _endGameScreen;

    /// <summary>
    /// Initialize the game UI
    /// </summary>
    /// <param name="onReplayPress">On replay press callback</param>
    /// <param name="totalScore">Total score to show</param>
    public void Initialize(Action onReplayPress, int totalScore)
    {
        _replayButton.onClick.RemoveAllListeners();
        _replayButton.onClick.AddListener(() => onReplayPress?.Invoke());
        UpdateTotalScoreText(totalScore);
    }

    /// <summary>
    /// Turn on the end game screen (with replay button and total score)
    /// </summary>
    /// <param name="score">Game score to show</param>
    public void ShowEndGameScreen(int score)
    {
        _endGameScreen.SetActive(true);
        _gameScoreText.SetText($"Game Score\n{score}");
    }

    /// <summary>
    /// Hides the end game screen
    /// </summary>
    public void HideEndGameScreen()
    {
        _endGameScreen.SetActive(false);
    }

    /// <summary>
    /// Update the total score text.
    /// </summary>
    /// <param name="totalScore">New total score</param>
    public void UpdateTotalScoreText(int totalScore)
    {
        _totalScoreText.SetText($"Score: {totalScore}");
    }
}
