using StevenUniverse.FanGame.Util;
using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    [System.Serializable]
    public class Template : JsonBase<Template>
    {
        //Instance
        [SerializeField] protected string tileLayerName = "Main";
        [SerializeField] protected string tileModeName = "Normal";
        [SerializeField] protected bool isGrounded = false;

        public Templates.TileTemplate.Layer TileLayer
        {
            get { return Templates.TileTemplate.Layer.Get(tileLayerName); }
        }

        public string TileModeName
        {
            get { return tileModeName; }
            set { tileModeName = value; }
        }

        public bool IsGrounded
        {
            get { return isGrounded; }
            set { isGrounded = value; }
        }


        //Constructor
        public Template()
        {
        }
    }
}