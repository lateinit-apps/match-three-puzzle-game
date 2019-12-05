using UnityEngine;

public class Collectible : GamePiece
{
    public bool clearedByBomb = false;
    public bool clearedAtBottom = true;

    private void Start() => matchValue = MatchValue.None;
}
