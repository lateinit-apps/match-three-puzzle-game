using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int movesLeft = 30;
    public int scoreGoal = 10000;

    public ScreenFader screenFader;
    
    public Text levelNameText;
    public Text movesLeftText;

    private Board board;

    private bool isReadyToBegin = false;
    private bool isGameOver = false;
    private bool isWinner = false;

    private void Start()
    {
        board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();

        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        UpdateMoves();

        StartCoroutine(ExecuteGameLoop());
    }

    private IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine(StartGameRoutine());
        yield return StartCoroutine(PlayGameRoutine());
        yield return StartCoroutine(EndGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        while (!isReadyToBegin)
        {
            yield return null;
            yield return new WaitForSeconds(1f);

            isReadyToBegin = true;
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
            if (movesLeft == 0)
            {
                isGameOver = true;
                isWinner = false;
            }

            yield return null;
        }
    }

    private IEnumerator EndGameRoutine()
    {
        if (screenFader != null)
        {
            screenFader.FadeOn();
        }

        if (isWinner)
        {

        }
        else
        {

        }

        yield return null;
    }

    public void UpdateMoves()
    {
        if (movesLeftText != null)
        {
            movesLeftText.text = movesLeft.ToString();
        }
    }
}
