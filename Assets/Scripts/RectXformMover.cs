using System.Collections;

using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectXformMover : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 onscreenPosition;
    public Vector3 endPosition;

    public float timeToMove = 1f;

    private RectTransform rectXform;

    private bool isMoving;

    private void Awake() => rectXform = GetComponent<RectTransform>();

    private void Move(Vector3 startPosition, Vector3 endPosition, float timeToMove)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(startPosition, endPosition, timeToMove));
        }
    }

    private IEnumerator MoveRoutine(Vector3 startPosition, Vector3 endPosition, float timeToMove)
    {
        if (rectXform != null)
        {
            rectXform.anchoredPosition = startPosition;
        }

        bool reachedDestination = false;
        float elapsedTime = 0f;
        isMoving = true;

        while (!reachedDestination)
        {
            if (Vector3.Distance(rectXform.anchoredPosition, endPosition) < 0.01f)
            {
                reachedDestination = true;
                break;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);
            t = t * t * t * (t * (t * 6 - 15) + 10);

            if (rectXform != null)
            {
                rectXform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);
            }

            yield return null;
        }

        isMoving = false;
    }

    public void MoveOn() => Move(startPosition, onscreenPosition, timeToMove);

    public void MoveOff() => Move(onscreenPosition, endPosition, timeToMove);
}
