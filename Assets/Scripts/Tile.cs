using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    private Board board;

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;

        this.board = board;
    }
}
