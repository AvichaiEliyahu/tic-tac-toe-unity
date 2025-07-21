using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _replayButton;
    [SerializeField] private TextMeshProUGUI _gameScoreText;
    [SerializeField] private TextMeshProUGUI _totalScoreText;
    [SerializeField] private GameObject _endGameScreen;

    public void Initialize(Action onPress, int totalScore)
    {
        _replayButton.onClick.RemoveAllListeners();
        _replayButton.onClick.AddListener(() => onPress?.Invoke());
        UpdateTotalScoreText(totalScore);
    }

    public void ShowEndGameScreen(int score)
    {
        _endGameScreen.SetActive(true);
        _gameScoreText.SetText($"Game Score\n{score}");
    }

    public void HideEndGameScreen()
    {
        _endGameScreen.SetActive(false);
    }

    public void UpdateTotalScoreText(int totalScore)
    {
        _totalScoreText.SetText($"Score: {totalScore}");
    }
}
