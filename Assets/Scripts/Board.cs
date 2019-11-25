using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;

    public float swapTime = 0.5f;

    private Tile[,] allTiles;
    private GamePiece[,] allGamePieces;

    private Tile clickedTile;
    private Tile targetTile;

    private void Start()
    {
        allTiles = new Tile[width, height];
        allGamePieces = new GamePiece[width, height];

        SetupTiles();
        SetupCamera();

        FillRandom();
    }

    private void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile =
                    Instantiate<GameObject>(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tile.name = "Tile (" + i + "," + j + ")";

                allTiles[i, j] = tile.GetComponent<Tile>();

                tile.transform.parent = transform;

                allTiles[i, j].Init(i, j, this);
            }
        }
    }

    private void SetupCamera()
    {
        Camera.main.transform.position =
            new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;

        Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
    }

    private GameObject GetRandomGamePiece()
    {
        int randomIndex = Random.Range(0, gamePiecePrefabs.Length);

        if (gamePiecePrefabs[randomIndex] == null)
        {
            Debug.LogWarning("BOARD: " + randomIndex + "does not contain a valid GamePiece prefab!");
        }

        return gamePiecePrefabs[randomIndex];
    }

    private void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece =
                    Instantiate<GameObject>(GetRandomGamePiece(), Vector3.zero, Quaternion.identity);

                if (randomPiece != null)
                {
                    randomPiece.GetComponent<GamePiece>().Init(this);
                    randomPiece.transform.parent = transform;

                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
                }
            }
        }
    }

    private void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPiece = allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = allGamePieces[targetTile.xIndex, targetTile.yIndex];

        clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
    }

    private bool IsWithinBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    private bool IsNextTo(Tile start, Tile end) =>
        (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex) ||
        (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex);

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD: Invalid GamePiece!");
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        gamePiece.SetCoordinates(x, y);

        if (IsWithinBounds(x, y))
        {
            allGamePieces[x, y] = gamePiece;
        }
    }

    public void ClickTile(Tile tile)
    {
        if (clickedTile == null)
        {
            clickedTile = tile;
        }
    }

    public void DragToTile(Tile tile)
    {
        if (clickedTile != null && IsNextTo(tile, clickedTile))
        {
            targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (clickedTile != null && targetTile != null)
        {
            SwitchTiles(clickedTile, targetTile);

            clickedTile = null;
            targetTile = null;
        }
    }
}
