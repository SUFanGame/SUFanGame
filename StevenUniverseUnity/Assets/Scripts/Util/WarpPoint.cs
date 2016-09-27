using UnityEngine;

namespace StevenUniverse.FanGame.Util
{
    public class WarpPoint
    {
        private string name;
        private string scene;
        private Vector3 position;
        private int elevation;

        public WarpPoint(string name, string scene, Vector3 position, int elevation)
        {
            this.name = name;
            this.scene = scene;
            this.position = position;
            this.elevation = elevation;
        }

        public string Name
        {
            get { return name; }
        }

        public string Scene
        {
            get { return scene; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public int Elevation
        {
            get { return elevation; }
        }
    }
}