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
                    UIManager.Instance.messageWindow.ShowCollectionGoal();
                }
            }

            if (UIManager.Instance.screenFader != null)
            {
                UIManager.Instance.screenFader.FadeOff();
            }
        }

        while (!isReadyToBegin)
        {
            yield return null;
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
            if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                UIManager.Instance.messageWindow.ShowWinMessage();
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }
        }
        else
        {
            if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                UIManager.Instance.messageWindow.ShowLoseMessage();
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }
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
