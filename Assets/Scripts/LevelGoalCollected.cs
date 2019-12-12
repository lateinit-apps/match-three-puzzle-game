public class LevelGoalCollected : LevelGoal
{
    public CollectionGoal[] collectGoals;

    private bool AreGoalsComplete(CollectionGoal[] goals)
    {
        foreach (CollectionGoal goal in goals)
        {
            if (goal == null | goals == null)
            {
                return false;
            }

            if (goals.Length == 0)
            {
                return false;
            }

            if (goal.numberToCollect != 0)
            {
                return false;
            }
        }

        return true;
    }

    public void UpdateGoals(GamePiece pieceToCheck)
    {
        if (pieceToCheck != null)
        {
            foreach (CollectionGoal goal in collectGoals )
            {
                if (goal != null)
                {
                    goal.CollectPiece(pieceToCheck);
                }
            }
        }
    }

    public override bool IsGameOver()
    {
        if (AreGoalsComplete(collectGoals) && ScoreManager.Instance != null)
        {
            int maxScore = scoreGoals[scoreGoals.Length - 1];

            if (ScoreManager.Instance.CurrentScore >= maxScore)
            {
                return true;
            }
        }

        return movesLeft <= 0;
    }

    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return ScoreManager.Instance.CurrentScore >= scoreGoals[0] &&
                   AreGoalsComplete(collectGoals);
        }

        return false;
    }
}
