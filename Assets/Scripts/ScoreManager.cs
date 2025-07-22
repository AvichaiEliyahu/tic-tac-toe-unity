using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class for managing the game score according to the instructions
/// </summary>
public class ScoreManager
{
    private const int WIN_MIN_SCORE = 50;
    private const int WIN_MAX_SCORE = 100;
    private const int DRAW_MIN_SCORE = 2;
    private const int DRAW_MAX_SCORE = 49;
    private const int LOSE_SCORE = 1;

    private const int FAST_TURN_TIME = 10;
    private const int SLOW_TURN_TIME = 20;

    private readonly List<float> _reactionTimes = new();
    private float _turnStartTime;


    public ScoreManager(List<float> reactionTimes)
    {
        _reactionTimes = reactionTimes;
    }

    public ScoreManager()
    {
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
                minScore = WIN_MIN_SCORE;
                maxScore = WIN_MAX_SCORE;
                break;
            case GameResult.Draw:
                minScore = DRAW_MIN_SCORE;
                maxScore = DRAW_MAX_SCORE;
                break;
            case GameResult.Lose:
                return LOSE_SCORE;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null); // should not get here with any other game result!
        }

        float t = Mathf.InverseLerp(SLOW_TURN_TIME, FAST_TURN_TIME, averageReactionTime);
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
