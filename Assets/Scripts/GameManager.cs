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

    private void Start()
    {
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
        while (!isGameOver)
        {
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.CurrentScore >= levelGoal.scoreGoals[0])
                {
                    isGameOver = true;
                    isWinner = true;
                }
            }

            if (levelGoal.movesLeft == 0)
            {
                isGameOver = true;
                isWinner = false;
            }

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

        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
    }

    public void UpdateMoves()
    {
        levelGoal.movesLeft--;

        if (movesLeftText != null)
        {
            movesLeftText.text = levelGoal.movesLeft.ToString();
        }
    }

    public void BeginGame() => isReadyToBegin = true;

    public void ReloadScene() => isReadyToReload = true;
}
