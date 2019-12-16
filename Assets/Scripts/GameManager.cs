using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    private Board board;

    private bool isReadyToBegin = false;
    private bool isReadyToReload = false;
    private bool isWinner = false;
    private bool isGameOver = false;

    public bool IsGameOver
    {
        get => isGameOver;
        set => isGameOver = value;
    }

    private LevelGoal levelGoal;
    private LevelGoalCollected levelGoalCollected;

    public LevelGoal LevelGoal
    {
        get => levelGoal;
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.scoreMeter != null)
            {
                UIManager.Instance.scoreMeter.SetupStars(levelGoal);
            }

            if (UIManager.Instance.levelNameText != null)
            {
                Scene scene = SceneManager.GetActiveScene();
                UIManager.Instance.levelNameText.text = scene.name;
            }

            if (levelGoalCollected != null)
            {
                UIManager.Instance.EnableColletionGoalLayout(true);
                UIManager.Instance.SetupCollectionGoalLayout(levelGoalCollected.collectionGoals);
            }
            else
            {
                UIManager.Instance.EnableColletionGoalLayout(false);
            }

            bool userTimer = levelGoal.levelCounter == LevelCounter.Timer;

            UIManager.Instance.EnableTimer(userTimer);
            UIManager.Instance.EnableMovesCounter(!userTimer);
        }



        levelGoal.movesLeft++;

        UpdateMoves();

        StartCoroutine(ExecuteGameLoop());
    }

    public override void Awake()
    {
        base.Awake();

        levelGoal = GetComponent<LevelGoal>();
        levelGoalCollected = GetComponent<LevelGoalCollected>();

        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine(StartGameRoutine());
        yield return StartCoroutine(PlayGameRoutine());
        yield return StartCoroutine(WaitForBoardRoutine(0.5f));
        yield return StartCoroutine(EndGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();

                int maxGoal = levelGoal.scoreGoals.Length - 1;
                UIManager.Instance.messageWindow.ShowScoreMessage(levelGoal.scoreGoals[maxGoal]);

                if (levelGoal.levelCounter == LevelCounter.Timer)
                {
                    UIManager.Instance.messageWindow.ShowTimedGoal(levelGoal.timeLeft);
                }
                else
                {
                    UIManager.Instance.messageWindow.ShowMovesGoal(levelGoal.movesLeft);
                }

                if (levelGoalCollected != null)
                {
                    UIManager.Instance.messageWindow.ShowCollectionGoal(true);

                    GameObject goalLayout = UIManager.Instance.messageWindow.collectionGoalLayout;

                    if (goalLayout != null)
                    {
                        UIManager.Instance.SetupCollectionGoalLayout(
                            levelGoalCollected.collectionGoals, goalLayout, 80);
                    }
                }
            }
        }

        while (!isReadyToBegin)
        {
            yield return null;
        }

        if (UIManager.Instance != null && UIManager.Instance.screenFader != null)
        {
            UIManager.Instance.screenFader.FadeOff();
        }

        yield return new WaitForSeconds(0.5f);

        if (board != null)
        {
            board.SetupBoard();
        }
    }

    private IEnumerator PlayGameRoutine()
    {
        if (levelGoal.levelCounter == LevelCounter.Timer)
        {
            levelGoal.StartCountdown();
        }

        while (!isGameOver)
        {
            isWinner = levelGoal.IsWinner();
            isGameOver = levelGoal.IsGameOver();

            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        isReadyToReload = false;

        if (isWinner)
        {
            ShowWinScreen();
        }
        else
        {
            ShowLoseScreen();
        }

        yield return new WaitForSeconds(1f);

        if (UIManager.Instance != null && UIManager.Instance.screenFader != null)
        {
            UIManager.Instance.screenFader.FadeOn();
        }

        while (!isReadyToReload)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if (levelGoal.levelCounter == LevelCounter.Timer &&
            UIManager.Instance != null && UIManager.Instance.timer != null)
        {
            UIManager.Instance.timer.FadeOff();
            UIManager.Instance.timer.paused = true;
        }

        if (board != null)
        {
            yield return new WaitForSeconds(board.swapTime);

            while (board.isRefilling)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(delay);
    }

    private void ShowWinScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
        {
            UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance.messageWindow.ShowWinMessage();
            UIManager.Instance.messageWindow.ShowCollectionGoal(false);

            if (ScoreManager.Instance != null)
            {
                string scoreString = "you scored\n" +
                    ScoreManager.Instance.CurrentScore.ToString() + " points!";

                UIManager.Instance.messageWindow.ShowGoalCaption(scoreString, 0, 70);
            }

            if (UIManager.Instance.messageWindow.goalCompleteIcon != null)
            {
                UIManager.Instance.messageWindow.ShowGoalImage(
                    UIManager.Instance.messageWindow.goalCompleteIcon);
            }
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWinSound();
        }
    }

    private void ShowLoseScreen()
    {
        if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
        {
            UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
            UIManager.Instance.messageWindow.ShowLoseMessage();
            UIManager.Instance.messageWindow.ShowCollectionGoal(false);

            string caption = "";

            if (levelGoal.levelCounter == LevelCounter.Timer)
            {
                caption = "Out of time!";
            }
            else
            {
                caption = "Out of moves!";
            }

            UIManager.Instance.messageWindow.ShowGoalCaption(caption, 0, 70);

            if (UIManager.Instance.messageWindow.goalFailedIcon != null)
            {
                UIManager.Instance.messageWindow.ShowGoalImage(
                    UIManager.Instance.messageWindow.goalFailedIcon);
            }
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLoseSound();
        }
    }

    public void UpdateMoves()
    {
        if (levelGoal.levelCounter == LevelCounter.Moves)
        {
            levelGoal.movesLeft--;

            if (UIManager.Instance != null && UIManager.Instance.movesLeftText != null)
            {
                UIManager.Instance.movesLeftText.text = levelGoal.movesLeft.ToString();
            }
        }
    }

    public void BeginGame() => isReadyToBegin = true;

    public void ReloadScene() => isReadyToReload = true;

    public void ScorePoints(GamePiece piece, int multiplier = 1, int bonus = 0)
    {
        if (piece != null)
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(piece.scoreValue * multiplier + bonus);

                levelGoal.UpdateScoreStars(ScoreManager.Instance.CurrentScore);

                if (UIManager.Instance != null && UIManager.Instance.scoreMeter != null)
                {
                    UIManager.Instance.scoreMeter.UpdateScoreMeter(
                        ScoreManager.Instance.CurrentScore, levelGoal.scoreStars);
                }
            }

            if (SoundManager.Instance != null && piece.clearSound != null)
            {
                SoundManager.Instance.PlayClipAtPoint(piece.clearSound, Vector3.zero,
                                                      SoundManager.Instance.fxVolume);
            }
        }
    }

    public void AddTime(int timeValue)
    {
        if (levelGoal.levelCounter == LevelCounter.Timer)
        {
            levelGoal.AddTime(timeValue);
        }
    }

    public void UpdateCollectionGoals(GamePiece pieceToCheck)
    {
        if (levelGoalCollected != null)
        {
            levelGoalCollected.UpdateGoals(pieceToCheck);
        }
    }
}
