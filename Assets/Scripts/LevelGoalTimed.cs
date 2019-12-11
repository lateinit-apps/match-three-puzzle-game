using System.Collections;

using UnityEngine;

public class LevelGoalTimed : LevelGoal
{
    public Timer timer;

    private int maxTime;

    private void Start()
    {
        if (timer != null)
        {
            timer.InitTimer(timeLeft);
        }

        maxTime = timeLeft;
    }

    private IEnumerator CountdownRoutine()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft--;

            if (timer != null)
            {
                timer.UpdateTimer(timeLeft);
            }
        }
    }

    public void StartCountdown() => StartCoroutine(CountdownRoutine());

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

    public void AddTime(int timeValue)
    {
        timeLeft += timeValue;
        timeLeft = Mathf.Clamp(timeLeft, 0, maxTime);

        if (timer != null)
        {
            timer.UpdateTimer(timeLeft);
        }
    }
}
