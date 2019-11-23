using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject tilePrefab;

    Tile[,] allTiles;

    private void Start()
    {
        allTiles = new Tile[width, height];
        SetupTiles();
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
            }
        }
    }
}
