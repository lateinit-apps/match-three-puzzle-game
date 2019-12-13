public class LevelGoalCollected : LevelGoal
{
    public CollectionGoal[] collectionGoals;
    public CollectionGoalPanel[] uiPanels;

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
            foreach (CollectionGoal goal in collectionGoals )
            {
                if (goal != null)
                {
                    goal.CollectPiece(pieceToCheck);
                }
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (CollectionGoalPanel panel in uiPanels)
        {
            if (panel != null)
            {
                panel.UpdatePanel();
            }
        }
    }

    public override bool IsGameOver()
    {
        if (AreGoalsComplete(collectionGoals) && ScoreManager.Instance != null)
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
                   AreGoalsComplete(collectionGoals);
        }

        return false;
    }
}
