using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public void SetCoordinates(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}
