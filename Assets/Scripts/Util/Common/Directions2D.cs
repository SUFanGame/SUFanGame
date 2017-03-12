

namespace SUGame.Util.Common
{
    public class Directions2D
    {
        public static IntVector3[] Quadrilateral = new IntVector3[]
        {
            IntVector3.left,
            IntVector3.up,
            IntVector3.right,
            IntVector3.down
        };

        public static IntVector3[] Octogonal = new IntVector3[]
        {
            Quadrilateral[0],
            Quadrilateral[1],
            Quadrilateral[2],
            Quadrilateral[3],
            IntVector3.left + IntVector3.up, // Top Left
            IntVector3.right + IntVector3.up, // Top right
            IntVector3.right + IntVector3.down, // Bottom right
            IntVector3.left + IntVector3.down, // Bottom left
        };

        public static int IndexOf( IntVector3 dir )
        {
            if (dir == Quadrilateral[0] ) return 0;
            if (dir == Quadrilateral[1]) return 1;
            if (dir == Quadrilateral[2]) return 2;
            if (dir == Quadrilateral[3]) return 3;
            if (dir == Octogonal[4]) return 4;
            if (dir == Octogonal[5]) return 5;
            if (dir == Octogonal[6]) return 6;
            if (dir == Octogonal[7]) return 7;

            return -1;
        }
    }
}
