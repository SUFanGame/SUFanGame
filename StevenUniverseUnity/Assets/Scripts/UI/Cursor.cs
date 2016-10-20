using UnityEngine;

namespace StevenUniverse.FanGame.UI
{
    public class MapCursor
    {
        private Vector3 truePosition;

        public Vector3 TruePosition
        {
            get { return truePosition; }
            set { truePosition = value; }
        }

        public MapCursor()
        {

        }
    }
}
