using UnityEngine;

public enum BombType
{
    None,
    Column,
    Row,
    Adjacent,
    Color
}

public class Bomb : GamePiece
{
    public BombType bombType;
}
