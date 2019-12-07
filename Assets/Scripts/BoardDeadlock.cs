using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class BoardDeadlock : MonoBehaviour
{
    private List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y,
                                       int listLength = 3, bool checkRow = true)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        List<GamePiece> piecesList = new List<GamePiece>();

        for (int i = 0; i < listLength; i++)
        {
            if (checkRow)
            {
                if (x + i < width && y < height && allPieces[x + i, y] != null)
                {
                    piecesList.Add(allPieces[x + i, y]);
                }
            }
            else
            {
                if (x < width && y + i < height && allPieces[x, y + i] != null)
                {
                    piecesList.Add(allPieces[x, y + i]);
                }
            }
        }

        return piecesList;
    }

    private List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minForMatch = 2)
    {
        List<GamePiece> matches = new List<GamePiece>();

        var groups = gamePieces.GroupBy(n => n.matchValue);

        foreach (var group in groups)
        {
            if (group.Count() >= minForMatch && group.Key != MatchValue.None)
            {
                matches = group.ToList();
            }
        }

        return matches;
    }

    private List<GamePiece> GetNeighbours(GamePiece[,] allPieces, int x, int y)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        List<GamePiece> neighbours = new List<GamePiece>();

        Vector2[] searchDirections = new Vector2[4]
        {
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0f, -1f)
        };

        foreach (Vector2 direction in searchDirections)
        {
            if (x + (int)direction.x >= 0 && x + (int)direction.x < width &&
                y + (int)direction.y >= 0 && y + (int)direction.y < height)
            {
                if (allPieces[x + (int)direction.x, y + (int)direction.y] != null)
                {
                    if (!neighbours.Contains(allPieces[x + (int)direction.x, y + (int)direction.y]))
                    {
                        neighbours.Add(allPieces[x + (int)direction.x, y + (int)direction.y]);
                    }
                }
            }
        }

        return neighbours;
    }

    private bool HasMoveAt(GamePiece[,] allPieces, int x, int y,
                           int listLength = 3, bool checkRow = true)
    {
        List<GamePiece> pieces = GetRowOrColumnList(allPieces, x, y, listLength, checkRow);
        List<GamePiece> matches = GetMinimumMatches(pieces, listLength - 1);

        GamePiece unmatchedPiece = null;

        if (pieces != null && matches != null)
        {
            if (pieces.Count == listLength && matches.Count == listLength - 1)
            {
                unmatchedPiece = pieces.Except(matches).FirstOrDefault();
            }

            if (unmatchedPiece != null)
            {
                List<GamePiece> neighbours =
                    GetNeighbours(allPieces, unmatchedPiece.xIndex, unmatchedPiece.yIndex);

                neighbours = neighbours.Except(matches).ToList();
                neighbours = neighbours.FindAll(n => n.matchValue == matches[0].matchValue);

                matches = matches.Union(neighbours).ToList();
            }

            if (matches.Count >= listLength)
            {
                string rowColumnString = checkRow ? "row" : "column";

                // Debug.Log("Available move: " + matches[0].matchValue + " piece to " +
                //           unmatchedPiece.xIndex + ", " + unmatchedPiece.yIndex +
                //           " to form matching " + rowColumnString);

                return true;
            }
        }

        return false;
    }

    public bool IsDeadlocked(GamePiece[,] allPieces, int listLength = 3)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        bool isDeadlocked = true;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (HasMoveAt(allPieces, i, j, listLength, true) ||
                    HasMoveAt(allPieces, i, j, listLength, false))
                {
                    isDeadlocked = false;
                }
            }
        }

        if (isDeadlocked)
        {
            Debug.Log("Board is deadlocked!");
        }

        return isDeadlocked;
    }
}
