using System.Collections;

using UnityEngine;

public enum MatchValue
{
    Blue,
    Cyan,
    Green,
    Indigo,
    Magenta,
    Purple,
    Orange,
    Red,
    Teal,
    Yellow,
    Wild,
    None
}

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public InterpolationType interpolation = InterpolationType.SmootherStep;

    public enum InterpolationType
    {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep
    }

    public MatchValue matchValue;

    public int scoreValue = 20;

    private bool isMoving = false;

    private Board board;

    public AudioClip clearSound;

    private IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {
        Vector3 startPosition = transform.position;

        bool reachedDestination = false;

        float elapsedTime = 0f;

        isMoving = true;

        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;

                if (board != null)
                {
                    board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
                }

                break;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            switch (interpolation)
            {
                case InterpolationType.Linear:
                    break;
                case InterpolationType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpolationType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpolationType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }
            t = t * t * (3 - 2 * t);

            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }

        isMoving = false;
    }

    public void ChangeColor(GamePiece pieceToMatch)
    {
        SpriteRenderer rendererToChange = GetComponent<SpriteRenderer>();

        Color colorToMatch = Color.clear;

        if (pieceToMatch != null)
        {
            SpriteRenderer rendererToMatch = pieceToMatch.GetComponent<SpriteRenderer>();

            if (rendererToMatch != null && rendererToChange != null)
            {
                rendererToChange.color = rendererToMatch.color;
            }

            matchValue = pieceToMatch.matchValue;
        }
    }

    public void Init(Board board) => this.board = board;

    public void SetCoordinates(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void Move(int destinationX, int destinationY, float timeToMove)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3(destinationX, destinationY, 0), timeToMove));
        }
    }

    public void ScorePoints(int multiplier = 1, int bonus = 0)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue * multiplier + bonus);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClipAtPoint(clearSound, Vector3.zero,
                                                  SoundManager.Instance.fxVolume);
        }
    }
}
