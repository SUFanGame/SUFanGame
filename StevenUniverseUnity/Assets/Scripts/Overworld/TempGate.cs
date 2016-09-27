using UnityEngine;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Overworld
{
    public class TempGate : MonoBehaviour
    {
        public string scene;
        public Vector3 position;
        public int elevation;

        public WarpPoint WarpPoint
        {
            get { return new WarpPoint("", scene, position, elevation); }
        }
    }
}