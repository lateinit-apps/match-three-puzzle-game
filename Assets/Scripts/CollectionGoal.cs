using UnityEngine;

public class CollectionGoal : MonoBehaviour
{
    public GamePiece prefabToCollect;

    [Range(1, 50)]
    public int numberToCollect = 5;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (prefabToCollect != null)
        {
            spriteRenderer = prefabToCollect.GetComponent<SpriteRenderer>();
        }
    }

    public void CollectPiece(GamePiece piece)
    {
        if (piece != null)
        {
            SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();

            if (this.spriteRenderer.sprite == spriteRenderer.sprite &&
                prefabToCollect.matchValue == piece.matchValue)
            {
                numberToCollect--;
                numberToCollect = Mathf.Clamp(numberToCollect, 0, numberToCollect);
            }
        }
    }
}
