using System.Collections;

using UnityEngine;

public enum TileType
{
    Normal,
    Obstacle,
    Breakable
}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public TileType tileType = TileType.Normal;

    private Board board;
    private SpriteRenderer spriteRenderer;

    public int breakableValue = 0;
    public Sprite[] breakableSprites;

    public Color normalColor;

    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

    private void OnMouseDown()
    {
        if (board != null)
        {
            board.ClickTile(this);
        }
    }

    private void OnMouseEnter()
    {
        if (board != null)
        {
            board.DragToTile(this);
        }
    }

    private void OnMouseUp()
    {
        if (board != null)
        {
            board.ReleaseTile();
        }
    }

    private IEnumerator BreakTileRoutine()
    {
        breakableValue = Mathf.Clamp(breakableValue--, 0, breakableValue);

        yield return new WaitForSeconds(0.25f);

        if (breakableSprites[breakableValue] != null)
        {
            spriteRenderer.sprite = breakableSprites[breakableValue];
        }

        if (breakableValue == 0)
        {
            tileType = TileType.Normal;
            spriteRenderer.color = normalColor;
        }
    }

    public void Init(int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;

        this.board = board;

        if (tileType == TileType.Breakable)
        {
            if (breakableSprites[breakableValue] != null)
            {
                spriteRenderer.sprite = breakableSprites[breakableValue];
            }
        }
    }

    public void BreakTile()
    {
        if (tileType != TileType.Breakable)
        {
            return;
        }

        StartCoroutine(BreakTileRoutine());
    }
}
