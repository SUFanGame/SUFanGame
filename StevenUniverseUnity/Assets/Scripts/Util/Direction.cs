using UnityEngine;

namespace StevenUniverse.FanGame.Util
{
    public class Direction : EnhancedEnum<Direction>
    {
        //Static
        private static Direction down;
        private static Direction up;
        private static Direction left;
        private static Direction right;

        //Static Instances
        static Direction()
        {
            Add(new Direction("Down", "Up", new Vector3(0, -1, 0)));
            Add(new Direction("Up", "Down", new Vector3(0, 1, 0)));
            Add(new Direction("Left", "Right", new Vector3(-1, 0, 0)));
            Add(new Direction("Right", "Left", new Vector3(1, 0, 0)));
        }

        //TODO use this pattern on all enhanced enums
        public static Direction Down
        {
            get
            {
                if (down == null)
                {
                    down = Direction.Get("Down");
                }
                return down;
            }
        }

        public static Direction Up
        {
            get
            {
                if (up == null)
                {
                    up = Direction.Get("Up");
                }
                return up;
            }
        }

        public static Direction Left
        {
            get
            {
                if (left == null)
                {
                    left = Direction.Get("Left");
                }
                return left;
            }
        }

        public static Direction Right
        {
            get
            {
                if (right == null)
                {
                    right = Direction.Get("Right");
                }
                return right;
            }
        }

        public static Direction GetFromVector(Vector3 directionVector)
        {
            foreach (Direction direction in Direction.Instances)
            {
                if (directionVector == direction.vector)
                {
                    return direction;
                }
            }

            return null;
        }

        //Instance
        private string oppositeDirectionName;
        private Vector3 vector;

        private Direction(string name, string oppositeDirectionName, Vector3 vector) : base(name)
        {
            this.oppositeDirectionName = oppositeDirectionName;
            this.vector = vector;
        }

        public Direction Opposite
        {
            get { return Direction.Get(oppositeDirectionName); }
        }

        public Vector3 Vector
        {
            get { return vector; }
        }
    }
}