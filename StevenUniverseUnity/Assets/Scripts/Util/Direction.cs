using UnityEngine;

namespace StevenUniverse.FanGame.Util
{
    public enum Direction
    {
        Down,
        Up,
        Left,
        Right
    }

    public static class DirectionExtensions
    {
        public static Direction GetOppositeDirection(this Direction direction)
        {
            switch (direction)
            {
            case Direction.Down: return Direction.Up;
            case Direction.Up: return Direction.Down;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: throw new UnityException("Invalid Direction!");
            }
        }

        public static Vector3 GetVector(this Direction direction)
        {
            switch (direction)
            {
            case Direction.Down: return new Vector3(0, -1, 0);
            case Direction.Up: return new Vector3(0, 1, 0);
            case Direction.Left: return new Vector3(-1, 0, 0);
            case Direction.Right: return new Vector3(1, 0, 0);
            default: throw new UnityException("Invalid Direction!");
            }
        }
    }
}