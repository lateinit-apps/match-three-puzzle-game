using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    // public int movesLeft = 30;
    // public int scoreGoal = 10000;


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

    public Sprite loseIcon;
    public Sprite winIcon;
    public Sprite goalIcon;

    private LevelGoal levelGoal;
    private LevelGoalTimed levelGoalTimed;
    private LevelGoalCollected levelGoalCollected;

    public LevelGoalTimed LevelGoalTimed
    {
        get => levelGoalTimed;
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

        }

        if (levelGoalCollected != null && UIManager.Instance != null)
        {
            UIManager.Instance.SetupCollectionGoalLayout(levelGoalCollected.collectionGoals);
        }

        levelGoal.movesLeft++;

        UpdateMoves();

        StartCoroutine(ExecuteGameLoop());
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
                UIManager.Instance.messageWindow.ShowMessage(goalIcon,
                                          "score goal\n" + levelGoal.scoreGoals[0].ToString(), "start");
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
        if (levelGoalTimed != null)
        {
            levelGoalTimed.StartCountdown();
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
                UIManager.Instance.messageWindow.ShowMessage(winIcon, "YOU WIN!", "OK");
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
                UIManager.Instance.messageWindow.ShowMessage(loseIcon, "YOU LOSE!", "OK");
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
        if (levelGoalTimed != null)
        {
            if (levelGoalTimed.timer != null)
            {
                levelGoalTimed.timer.FadeOff();
                levelGoalTimed.timer.paused = true;
            }
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

    public override void Awake()
    {
        base.Awake();

        levelGoal = GetComponent<LevelGoal>();
        levelGoalTimed = GetComponent<LevelGoalTimed>();
        levelGoalCollected = GetComponent<LevelGoalCollected>();

        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
    }

    public void UpdateMoves()
    {
        if (levelGoalTimed == null)
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
        if (levelGoalTimed != null)
        {
            levelGoalTimed.AddTime(timeValue);
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
