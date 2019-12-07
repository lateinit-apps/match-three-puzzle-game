using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class BoardDeadlock : MonoBehaviour
{
    List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y,
                                       int listLength = 3, bool checkRow = true)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        List<GamePiece> piecesList = new List<GamePiece>();

        for (int i = 0; i < listLength; i++)
        {
            if (checkRow)
            {
                if (x + i < width && y < height)
                {
                    piecesList.Add(allPieces[x + i, y]);
                }
            }
            else
            {
                if (x < width && y + i < height)
                {
                    piecesList.Add(allPieces[x, y + i]);
                }
            }
        }

        return piecesList;
    }

    List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minForMatch = 2)
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

    List<GamePiece> GetNeighbours(GamePiece[,] allPieces, int x, int y)
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

}
