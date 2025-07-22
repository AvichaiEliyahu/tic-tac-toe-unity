using System;

[Serializable]
public class GameData
{
    public int BoardSize = 3;
    public BotData BotData;
    public ScoringSystemData ScoringSystemData;
}

[Serializable]
public class BotData
{
    public int BotMinDelay = 1;
    public int BotMaxDelay = 3;
}

[Serializable]
public class ScoringSystemData
{
    public int WinMinScore = 50;
    public int WinMaxScore = 100;
    public int DrawMinScore = 2;
    public int DrawMaxScore = 49;
    public int LoseScore = 1;
    public int FastTurnTime = 10;
    public int SlowTurnTime = 20;
}