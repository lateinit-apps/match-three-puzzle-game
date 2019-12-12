using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelGoal))]
public class GameManager : Singleton<GameManager>
{
    // public int movesLeft = 30;
    // public int scoreGoal = 10000;

    public ScreenFader screenFader;

    public Text levelNameText;
    public Text movesLeftText;

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

    public MessageWindow messageWindow;

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

    public ScoreMeter scoreMeter;

    private void Start()
    {
        if (scoreMeter != null)
        {
            scoreMeter.SetupStars(levelGoal);
        }

        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
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
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectXformMover>().MoveOn();
            messageWindow.ShowMessage(goalIcon,
                                      "score goal\n" + levelGoal.scoreGoals[0].ToString(), "start");
        }

        while (!isReadyToBegin)
        {
            yield return null;
        }

        if (screenFader != null)
        {
            screenFader.FadeOff();
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
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(winIcon, "YOU WIN!", "OK");
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }
        }
        else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(loseIcon, "YOU LOSE!", "OK");
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }
        }

        yield return new WaitForSeconds(1f);

        if (screenFader != null)
        {
            screenFader.FadeOn();
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

            if (movesLeftText != null)
            {
                movesLeftText.text = levelGoal.movesLeft.ToString();
            }
        }
        else
        {
            if (movesLeftText != null)
            {
                movesLeftText.text = "\u221E";
                movesLeftText.fontSize = 70;
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

                if (scoreMeter != null)
                {
                    scoreMeter.UpdateScoreMeter(
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
