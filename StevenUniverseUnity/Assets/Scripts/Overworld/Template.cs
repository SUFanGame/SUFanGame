using StevenUniverse.FanGame.Util;
using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    [System.Serializable]
    public class Template : JsonBase<Template>
    {
        //Instance
        [SerializeField] protected string tileLayerName = "Main";

        public Templates.TileTemplate.Layer TileLayer
        {
            get { return Templates.TileTemplate.Layer.Get(tileLayerName); }
        }


        //Constructor
        public Template()
        {
        }
    }
}