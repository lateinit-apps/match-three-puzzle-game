using System.Collections.Generic;

using UnityEngine;

public class BoardShuffler : MonoBehaviour
{
    public List<GamePiece> RemoveNormalPieces(GamePiece[,] allPieces)
    {
        List<GamePiece> normalPieces = new List<GamePiece>();

        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i, j] != null)
                {
                    Bomb bomb = allPieces[i, j].GetComponent<Bomb>();
                    Collectible collectible = allPieces[i, j].GetComponent<Collectible>();

                    if (bomb == null && collectible == null)
                    {
                        normalPieces.Add(allPieces[i, j]);
                        allPieces[i, j] = null;
                    }
                }
            }
        }

        return normalPieces;
    }

    public void ShuffleList(List<GamePiece> piecesToShuffle)
    {
        int maxCount = piecesToShuffle.Count;

        for (int i = 0; i < maxCount - 1; i++)
        {
            int r = Random.Range(i, maxCount);

            if (r == i)
            {
                continue;
            }

            GamePiece temp = piecesToShuffle[r];

            piecesToShuffle[r] = piecesToShuffle[i];
            piecesToShuffle[i] = temp;
        }
    }

    public void MovePieces(GamePiece[,] allPieces, float swapTime = 0.5f)
    {
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i, j] != null)
                {
                    allPieces[i, j].Move(i, j, swapTime);
                }
            }
        }
    }
}
