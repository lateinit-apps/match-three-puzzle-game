using UnityEngine;

public class LevelGoal : Singleton<LevelGoal>
{
    public int scoreStars = 0;
    public int[] scoreGoals = new int[3] { 1000, 2000, 3000 };

    public int movesLeft = 30;

    public void Init()
    {
        scoreStars = 0;

        for (int i = 0; i < scoreGoals.Length; i++)
        {
            if (scoreGoals[i] < scoreGoals[i - 1])
            {
                Debug.LogWarning("Level goal: setup score goals in increasing order!");
            }
        }
    }

    public int UpdateScore(int score)
    {
        for (int i = 0; i < scoreGoals.Length; i++)
        {
            if (score < scoreGoals[i])
            {
                return i;
            }
        }

        return scoreGoals.Length;
    }

    public void UpdateScoreStars(int score) => scoreStars = UpdateScore(score);
}
