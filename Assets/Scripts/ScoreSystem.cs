using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreSystem
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


    public ScoreSystem(List<float> reactionTimes)
    {
        _reactionTimes = reactionTimes;
    }

    public ScoreSystem()
    {
    }

    public void StartTurn()
    {
        _turnStartTime = Time.time;
    }

    public void EndTurn()
    {
        var turnDuration = Time.time - _turnStartTime;
        _reactionTimes.Add(turnDuration);
    }

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

    public List<float> GetCurrentReactionTimes()
    {
        return _reactionTimes;
    }
}
