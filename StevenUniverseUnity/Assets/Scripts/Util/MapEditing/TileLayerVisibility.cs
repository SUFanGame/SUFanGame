using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SUGame.World;

namespace SUGame.Util.MapEditing
{
    /// <summary>
    /// Class managing the visibility state of TileLayers.
    /// </summary>
    [System.Serializable]
    public class TileLayerVisibility
    {
        // Serializable list of the layers state
        [SerializeField]
        BoolList layerStates_ = new BoolList();

        private BoolList LayerStates_
        {
            get
            {
                //If the number of TileLayers has changed since the layerStates list was last update, recreate it
                int layerCount = EnumUtil.GetEnumValues<TileLayer>().Count;
                if (layerStates_.Count != layerCount)
                {
                    layerStates_ = new BoolList();

                    for (int i = 0; i < layerCount; ++i)
                    {
                        layerStates_.Add(true);
                    }
                }

                return layerStates_;
            }
        }

        public void Set( TileLayer layer, bool isVisible )
        {
            int i = (int)layer;
            LayerStates_[i] = isVisible;
        }

        public bool Get( TileLayer layer )
        {
            //Debug.LogFormat("Layer states after in get function: {0}", string.Join(",", layerStates_.Select(b => b.ToString()).ToArray()));
            int i = (int)layer;
            return LayerStates_[i];
        }

        public void SetAll( bool areVisible )
        {
            for (int i = 0; i < layerStates_.Count; ++i)
                LayerStates_[i] = areVisible;
        }

        [System.Serializable]
        class BoolList : List<bool> { }
    }
}
