using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class for managing the game score according to the instructions
/// </summary>
public class ScoreManager
{
    private readonly List<float> _reactionTimes = new();
    private float _turnStartTime;

    private ScoringSystemData _data;


    public ScoreManager(ScoringSystemData data, List<float> reactionTimes) : this(data)
    {
        _reactionTimes = reactionTimes;
    }

    public ScoreManager(ScoringSystemData data)
    {
        _data = data;
    }

    /// <summary>
    /// Records the time when the turn starts.
    /// </summary>
    public void StartTurn()
    {
        _turnStartTime = Time.time;
    }

    /// <summary>
    /// Records the time when the turn ends and add the total time to the reaction times list.
    /// </summary>
    public void EndTurn()
    {
        var turnDuration = Time.time - _turnStartTime;
        _reactionTimes.Add(turnDuration);
    }

    /// <summary>
    /// Calculates the total score based on the game result and the data recorded in the reaction times list
    /// </summary>
    /// <param name="result">game result, according to which the score is calculated</param>
    /// <returns>Total score</returns>
    /// <exception cref="ArgumentOutOfRangeException">For illegal game result type</exception>
    public int CalcFinalScore(GameResult result)
    {
        var averageReactionTime = _reactionTimes.Count > 0 ? _reactionTimes.Average() : 0;
        int minScore, maxScore;

        switch (result)
        {
            case GameResult.Win:
                minScore = _data.WinMinScore;
                maxScore = _data.WinMaxScore;
                break;
            case GameResult.Draw:
                minScore = _data.DrawMinScore;
                maxScore = _data.DrawMaxScore;
                break;
            case GameResult.Lose:
                return _data.LoseScore;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null); // should not get here with any other game result!
        }

        float t = Mathf.InverseLerp(_data.SlowTurnTime, _data.FastTurnTime, averageReactionTime);
        return Mathf.RoundToInt(Mathf.Lerp(minScore, maxScore, t));
    }

    /// <summary>
    /// Rerturns the list of reaction times.
    /// </summary>
    /// <returns></returns>
    public List<float> GetCurrentReactionTimes()
    {
        return _reactionTimes;
    }
}
