using System.Collections;

using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    private bool isMoving = false;

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
                transform.position = destination;
                SetCoordinates((int)destination.x, (int)destination.y);
                break;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }

        isMoving = false;
    }

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
}
