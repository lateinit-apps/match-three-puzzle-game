using UnityEngine;

public class LevelGoalTimed : LevelGoal
{
    public override void Start()
    {
        levelCounter = LevelCounter.Timer;

        base.Start();

        if (UIManager.Instance != null && UIManager.Instance.timer != null)
        {
            UIManager.Instance.timer.InitTimer(timeLeft);
        }
    }

    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return ScoreManager.Instance.CurrentScore >= scoreGoals[0];
        }

        return false;
    }

    public override bool IsGameOver()
    {
        int maxScore = scoreGoals[scoreGoals.Length - 1];

        if (ScoreManager.Instance.CurrentScore > maxScore)
        {
            return true;
        }
        
        return timeLeft <= 0;
    }
}
