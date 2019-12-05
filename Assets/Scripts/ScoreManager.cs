using System.Collections;

using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    private int currentScore = 0;
    public int CurrentScore
    {
        get => currentScore;
    }

    private int counterValue = 0;
    private int increment = 5;

    public Text scoreText;

    private void Start() => UpdateScoreText(currentScore);

    private IEnumerator CountScoreRoutine()
    {
        int iterations = 0;

        while (counterValue < currentScore && iterations < 1000000)
        {
            counterValue += increment;
            UpdateScoreText(counterValue);
            iterations++;

            yield return null;
        }

        counterValue = currentScore;
        UpdateScoreText(currentScore);
    }

    public void UpdateScoreText(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int value)
    {
        currentScore += value;
        StartCoroutine(CountScoreRoutine());
    }
}
