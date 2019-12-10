using UnityEngine;

public abstract class LevelGoal : Singleton<LevelGoal>
{
    public int scoreStars = 0;
    public int[] scoreGoals = new int[3] { 1000, 2000, 3000 };

    public int movesLeft = 30;
    public int timeLeft = 60;

    private void Start() => Init();

    private int UpdateScore(int score)
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

    public void Init()
    {
        scoreStars = 0;

        for (int i = 1; i < scoreGoals.Length; i++)
        {
            if (scoreGoals[i] < scoreGoals[i - 1])
            {
                Debug.LogWarning("Level goal: setup score goals in increasing order!");
            }
        }
    }

    public void UpdateScoreStars(int score) => scoreStars = UpdateScore(score);

    public abstract bool IsWinner();
    
    public abstract bool IsGameOver();
}
