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

    public void Initialize(Action onPress)
    {
        _replayButton.onClick.RemoveAllListeners();
        _replayButton.onClick.AddListener(() => onPress?.Invoke());
    }

    public void ShowReplayView(int score)
    {
        _endGameScreen.SetActive(true);
        _gameScoreText.SetText($"Game Score\n{score}");
    }

    public void HideReplayView()
    {
        _endGameScreen.SetActive(false);
    }

}
