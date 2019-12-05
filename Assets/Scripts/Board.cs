using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public int borderSize;

    public GameObject tileNormalPrefab;
    public GameObject tileObstaclePrefab;
    public GameObject[] gamePiecePrefabs;

    public GameObject adjacentBombPrefab;
    public GameObject columnBombPrefab;
    public GameObject rowBombPrefab;
    public GameObject colorBombPrefab;

    private GameObject clickedTileBomb;
    private GameObject targetTileBomb;

    public int maxCollectibles = 3;
    public int collectibleCount = 0;

    [Range(0, 1)]
    public float chanceForCollectible = 0.1f;

    public GameObject[] collectiblePrefabs;

    public float swapTime = 0.5f;

    private Tile[,] allTiles;
    private GamePiece[,] allGamePieces;

    private Tile clickedTile;
    private Tile targetTile;

    private bool playerInputEnabled = true;

    public StartingObject[] startingTiles;
    public StartingObject[] startingGamePieces;

    private ParticleManager particleManager;

    public int fillYOffset = 10;
    public float fillMoveTime = 0.5f;

    [System.Serializable]
    public class StartingObject
    {
        public GameObject prefab;

        public int x;
        public int y;
        public int z;
    }

    private void Start()
    {
        allTiles = new Tile[width, height];
        allGamePieces = new GamePiece[width, height];

        SetupTiles();
        SetupGamePieces();

        List<GamePiece> startingCollectibles = FindAllCollectibles();
        collectibleCount = startingCollectibles.Count;

        SetupCamera();

        FillBoard(fillYOffset, fillMoveTime);

        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }

    private void MakeTile(GameObject prefab, int x, int y, int z = 0)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject tile =
                Instantiate<GameObject>(prefab, new Vector3(x, y, z), Quaternion.identity);
            tile.name = "Tile (" + x + "," + y + ")";

            allTiles[x, y] = tile.GetComponent<Tile>();

            tile.transform.parent = transform;

            allTiles[x, y].Init(x, y, this);
        }
    }

    private void MakeGamePiece(GameObject prefab, int x, int y,
                               int falseYOffest = 0, float moveTime = 0.1f)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            prefab.GetComponent<GamePiece>().Init(this);
            prefab.transform.parent = transform;

            PlaceGamePiece(prefab.GetComponent<GamePiece>(), x, y);

            if (falseYOffest != 0)
            {
                prefab.transform.position = new Vector3(x, y + falseYOffest, 0);
                prefab.GetComponent<GamePiece>().Move(x, y, moveTime);
            }

        }
    }

    private GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject bomb =
                Instantiate<GameObject>(prefab, new Vector3(x, y, 0), Quaternion.identity);

            bomb.GetComponent<Bomb>().Init(this);
            bomb.GetComponent<Bomb>().SetCoordinates(x, y);
            bomb.transform.parent = transform;

            return bomb;
        }

        return null;
    }

    private void SetupTiles()
    {
        foreach (StartingObject startingTile in startingTiles)
        {
            if (startingTiles != null)
            {
                MakeTile(startingTile.prefab, startingTile.x, startingTile.y, startingTile.z);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] == null)
                {
                    MakeTile(tileNormalPrefab, i, j);
                }
            }
        }
    }

    private void SetupGamePieces()
    {
        foreach (StartingObject startingPiece in startingGamePieces)
        {
            if (startingPiece != null)
            {
                GameObject piece =
                    Instantiate<GameObject>(startingPiece.prefab,
                                            new Vector3(startingPiece.x, startingPiece.y, 0),
                                            Quaternion.identity);

                MakeGamePiece(piece, startingPiece.x, startingPiece.y, fillYOffset, fillMoveTime);
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

    private GameObject GetRandomObject(GameObject[] objectArray)
    {
        int randomIndex = Random.Range(0, objectArray.Length);

        if (objectArray[randomIndex] == null)
        {
            Debug.LogWarning("Board.GetRandomObject at index " + randomIndex +
                             " does not contain a valid GameObject!");
        }

        return objectArray[randomIndex];
    }

    private GameObject GetRandomGamePiece() => GetRandomObject(gamePiecePrefabs);

    private GameObject GetRandomCollectible() => GetRandomObject(collectiblePrefabs);

    private GamePiece FillRandomGamePieceAt(int x, int y,
                                            int falseYOffest = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomPiece =
               Instantiate<GameObject>(GetRandomGamePiece(), Vector3.zero, Quaternion.identity);

            MakeGamePiece(randomPiece, x, y, falseYOffest, moveTime);

            return randomPiece.GetComponent<GamePiece>();
        }

        return null;
    }

    private GamePiece FillRandomCollectibleAt(int x, int y,
                                              int falseYOffest = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomPiece =
               Instantiate<GameObject>(GetRandomCollectible(), Vector3.zero, Quaternion.identity);

            MakeGamePiece(randomPiece, x, y, falseYOffest, moveTime);

            return randomPiece.GetComponent<GamePiece>();
        }

        return null;
    }


    private void FillBoard(int falseYOffest = 0, float moveTime = 0.1f)
    {
        int maxIterations = 100;
        int iterations;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGamePieces[i, j] == null && allTiles[i, j].tileType != TileType.Obstacle)
                {
                    GamePiece piece = null;

                    if (j == height - 1 && CanAddCollectible())
                    {
                        piece = FillRandomCollectibleAt(i, j, falseYOffest, moveTime);
                        collectibleCount++;
                    }
                    else
                    {
                        piece = FillRandomGamePieceAt(i, j, falseYOffest, moveTime);
                        iterations = 0;

                        while (HasMatchOnFill(i, j))
                        {
                            ClearPieceAt(i, j);
                            piece = FillRandomGamePieceAt(i, j, falseYOffest, moveTime);
                            iterations++;

                            if (iterations >= maxIterations)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downwardMatches = FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        return leftMatches.Count > 0 || downwardMatches.Count > 0;
    }

    private void SwitchTiles(Tile clickedTile, Tile targetTile) =>
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));

    private IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        if (playerInputEnabled)
        {
            GamePiece clickedPiece = allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
            GamePiece targetPiece = allGamePieces[targetTile.xIndex, targetTile.yIndex];

            if (clickedPiece != null && targetPiece != null)
            {
                clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

                yield return new WaitForSeconds(swapTime);

                List<GamePiece> clickedPieceMatches =
                    FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<GamePiece> targetPieceMatches =
                    FindMatchesAt(targetTile.xIndex, targetTile.yIndex);
                List<GamePiece> colorMatches = new List<GamePiece>();

                if (IsColorBomb(clickedPiece) && !IsColorBomb(targetPiece))
                {
                    clickedPiece.matchValue = targetPiece.matchValue;
                    colorMatches = FindAllMatchValue(clickedPiece.matchValue);
                }
                else if (!IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
                {
                    targetPiece.matchValue = clickedPiece.matchValue;
                    colorMatches = FindAllMatchValue(targetPiece.matchValue);
                }
                else if (IsColorBomb(clickedPiece) && IsColorBomb(targetPiece))
                {
                    foreach (GamePiece piece in allGamePieces)
                    {
                        if (!colorMatches.Contains(piece))
                        {
                            colorMatches.Add(piece);
                        }
                    }
                }

                if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0 &&
                    colorMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                    targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                }
                else
                {
                    yield return new WaitForSeconds(swapTime);

                    Vector2 swapDirection = new Vector2(targetTile.xIndex - clickedTile.xIndex,
                                                        targetTile.yIndex - clickedTile.yIndex);

                    clickedTileBomb = DropBomb(clickedTile.xIndex, clickedTile.yIndex,
                                               swapDirection, clickedPieceMatches);
                    targetTileBomb = DropBomb(targetTile.xIndex, targetTile.yIndex,
                                              swapDirection, targetPieceMatches);

                    if (clickedTileBomb != null && targetPiece != null)
                    {
                        GamePiece clickedBombPiece = clickedTileBomb.GetComponent<GamePiece>();

                        if (!IsColorBomb(clickedBombPiece))
                        {
                            clickedBombPiece.ChangeColor(targetPiece);
                        }
                    }

                    if (targetTileBomb != null && clickedPiece != null)
                    {
                        GamePiece clickedBombPiece = targetTileBomb.GetComponent<GamePiece>();

                        if (!IsColorBomb(clickedBombPiece))
                        {
                            clickedBombPiece.ChangeColor(clickedPiece);
                        }
                    }


                    ClearAndRefillBoard(clickedPieceMatches
                                        .Union(targetPieceMatches).ToList()
                                        .Union(colorMatches).ToList());
                }
            }
        }
    }

    private bool IsWithinBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    private bool IsNextTo(Tile start, Tile end) =>
        (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex) ||
        (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex);

    private List<GamePiece> FindMatches(int startX, int startY,
                                        Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = width > height ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece) &&
                    nextPiece.matchValue != MatchValue.None)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }

        return null;
    }

    private List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return combinedMatches.Count >= minLength ? combinedMatches : null;
    }

    private List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return combinedMatches.Count >= minLength ? combinedMatches : null;
    }

    private List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizontalMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> verticalMatches = FindVerticalMatches(x, y, minLength);

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();
        }

        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();
        }

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();

        return combinedMatches;
    }

    private List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }

        return matches;
    }

    private List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<GamePiece> matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }

        return combinedMatches;
    }

    private List<GamePiece> FindAllMatchValue(MatchValue matchValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGamePieces[i, j] != null)
                {
                    if (allGamePieces[i, j].matchValue == matchValue)
                    {
                        foundPieces.Add(allGamePieces[i, j]);
                    }
                }
            }
        }

        return foundPieces;
    }

    private void HighlightTileOff(int x, int y)
    {
        if (allTiles[x, y].tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r,
                                             spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }

    private void HighlightTileOn(int x, int y, Color color)
    {
        if (allTiles[x, y].tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    private void HighlightMatchesAt(int x, int y)
    {
        HighlightTileOff(x, y);

        var combinedMatches = FindMatchesAt(x, y, 3);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex,
                                piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    private void HighlightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HighlightMatchesAt(i, j);
            }
        }
    }

    private void HighlightPieces(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex,
                                piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    private void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = allGamePieces[x, y];

        if (pieceToClear != null)
        {
            allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        // HighlightTileOff(x, y);
    }

    private void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombedPieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);

                if (particleManager != null)
                {
                    if (bombedPieces.Contains(piece))
                    {
                        particleManager.BombFXAt(piece.xIndex, piece.yIndex);
                    }
                    else
                    {
                        particleManager.ClearPieceFXAt(piece.xIndex, piece.yIndex);
                    }
                }
            }
        }
    }

    private void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    private void BreakTileAt(int x, int y)
    {
        Tile tileToBreak = allTiles[x, y];

        if (tileToBreak != null && tileToBreak.tileType == TileType.Breakable)
        {
            if (particleManager != null)
            {
                particleManager.BreakTileFXAt(tileToBreak.breakableValue, x, y, 0);
            }

            tileToBreak.BreakTile();
        }
    }

    private void BreakTileAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                BreakTileAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    private List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        for (int i = 0; i < height - 1; i++)
        {
            if (allGamePieces[column, i] == null &&
                allTiles[column, i].tileType != TileType.Obstacle)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (allGamePieces[column, j] != null)
                    {
                        allGamePieces[column, j].Move(column, i, collapseTime * (j - i));
                        allGamePieces[column, i] = allGamePieces[column, j];
                        allGamePieces[column, i].SetCoordinates(column, i);

                        if (!movingPieces.Contains(allGamePieces[column, i]))
                        {
                            movingPieces.Add(allGamePieces[column, i]);
                        }

                        allGamePieces[column, j] = null;

                        break;
                    }
                }
            }
        }

        return movingPieces;
    }

    private List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    private List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }

        return columns;
    }

    private void ClearAndRefillBoard(List<GamePiece> gamePieces) =>
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));

    private IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {
        playerInputEnabled = false;

        List<GamePiece> matches = gamePieces;

        do
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(gamePieces));
            yield return null;

            yield return StartCoroutine(RefillRoutine());

            matches = FindAllMatches();

            yield return new WaitForSeconds(0.25f);
        }
        while (matches.Count != 0);

        playerInputEnabled = true;
    }

    private IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();

        // HighlightPieces(gamePieces);

        yield return new WaitForSeconds(0.25f);

        bool isFinished = false;

        while (!isFinished)
        {
            List<GamePiece> bombedPieces = GetBombedPieces(gamePieces);

            gamePieces = gamePieces.Union(bombedPieces).ToList();

            bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            List<GamePiece> collectedPieces = FindCollectiblesAt(0);

            collectibleCount -= collectedPieces.Count;

            gamePieces = gamePieces.Union(collectedPieces).ToList();

            ClearPieceAt(gamePieces, bombedPieces);
            BreakTileAt(gamePieces);

            if (clickedTileBomb != null)
            {
                ActivateBomb(clickedTileBomb);
                clickedTileBomb = null;
            }

            if (targetTileBomb != null)
            {
                ActivateBomb(targetTileBomb);
                targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.25f);

            movingPieces = CollapseColumn(gamePieces);

            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            matches = FindMatchesAt(movingPieces);
            collectedPieces = FindCollectiblesAt(0);
            matches = matches.Union(collectedPieces).ToList();

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
    }

    private bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private IEnumerator RefillRoutine()
    {
        FillBoard(fillYOffset, fillMoveTime);

        yield return null;
    }

    private List<GamePiece> GetRowPieces(int row)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            if (allGamePieces[i, row] != null)
            {
                gamePieces.Add(allGamePieces[i, row]);
            }
        }

        return gamePieces;
    }

    private List<GamePiece> GetColumnPieces(int column)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            if (allGamePieces[column, i] != null)
            {
                gamePieces.Add(allGamePieces[column, i]);
            }
        }

        return gamePieces;
    }

    private List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    gamePieces.Add(allGamePieces[i, j]);
                }
            }
        }

        return gamePieces;
    }

    private List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
    {
        List<GamePiece> allPiecesToClear = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                List<GamePiece> piecesToClear = new List<GamePiece>();

                Bomb bomb = piece.GetComponent<Bomb>();

                if (bomb != null)
                {
                    switch (bomb.bombType)
                    {
                        case BombType.Column:
                            piecesToClear = GetColumnPieces(bomb.xIndex);
                            break;
                        case BombType.Row:
                            piecesToClear = GetRowPieces(bomb.yIndex);
                            break;
                        case BombType.Adjacent:
                            piecesToClear = GetAdjacentPieces(bomb.xIndex, bomb.yIndex, 1);
                            break;
                        case BombType.Color:
                            break;
                    }

                    allPiecesToClear = allPiecesToClear.Union(piecesToClear).ToList();
                    allPiecesToClear = RemoveCollectibles(allPiecesToClear);
                }
            }
        }

        return allPiecesToClear;
    }

    private bool IsCornerMatch(List<GamePiece> gamePieces)
    {
        bool vertical = false;
        bool horizontal = false;

        int xStart = -1;
        int yStart = -1;

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (xStart == -1 || yStart == -1)
                {
                    xStart = piece.xIndex;
                    yStart = piece.yIndex;

                    continue;
                }

                if (piece.xIndex != xStart && piece.yIndex == yStart)
                {
                    horizontal = true;
                }


                if (piece.xIndex == xStart && piece.yIndex != yStart)
                {
                    vertical = true;
                }
            }
        }

        return horizontal && vertical;
    }

    private GameObject DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> gamePieces)
    {
        GameObject bomb = null;

        if (gamePieces.Count >= 4)
        {
            if (IsCornerMatch(gamePieces))
            {
                if (adjacentBombPrefab != null)
                {
                    bomb = MakeBomb(adjacentBombPrefab, x, y);
                }
            }
            else
            {
                if (gamePieces.Count >= 5)
                {
                    if (colorBombPrefab != null)
                    {
                        bomb = MakeBomb(colorBombPrefab, x, y);
                    }
                }
                else
                if (swapDirection.x != 0)
                {
                    if (rowBombPrefab != null)
                    {
                        bomb = MakeBomb(rowBombPrefab, x, y);
                    }
                }
                else
                {
                    if (columnBombPrefab != null)
                    {
                        bomb = MakeBomb(columnBombPrefab, x, y);
                    }
                }
            }
        }

        return bomb;
    }

    private void ActivateBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;

        if (IsWithinBounds(x, y))
        {
            allGamePieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }

    private bool IsColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();

        if (bomb != null)
        {
            return bomb.bombType == BombType.Color;
        }

        return false;
    }

    private List<GamePiece> FindCollectiblesAt(int row)
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            if (allGamePieces[i, row] != null)
            {
                Collectible collectibleComponent = allGamePieces[i, row].GetComponent<Collectible>();

                if (collectibleComponent != null)
                {
                    foundCollectibles.Add(allGamePieces[i, row]);
                }
            }
        }

        return foundCollectibles;
    }

    private List<GamePiece> FindAllCollectibles()
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            List<GamePiece> collectibleRow = FindCollectiblesAt(i);
            foundCollectibles = foundCollectibles.Union(collectibleRow).ToList();
        }

        return foundCollectibles;
    }

    private bool CanAddCollectible() => Random.Range(0f, 1f) <= chanceForCollectible &&
        collectiblePrefabs.Length > 0 && collectibleCount < maxCollectibles;

    private List<GamePiece> RemoveCollectibles(List<GamePiece> bombedPieces)
    {
        List<GamePiece> collectiblePieces = FindAllCollectibles();
        List<GamePiece> piecesToRemove = new List<GamePiece>();

        foreach (GamePiece piece in collectiblePieces)
        {
            Collectible collectibleComponent = piece.GetComponent<Collectible>();

            if (collectibleComponent != null)
            {
                if (!collectibleComponent.clearedByBomb)
                {
                    piecesToRemove.Add(piece);
                }
            }
        }

        return bombedPieces.Except(piecesToRemove).ToList();
    }

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
