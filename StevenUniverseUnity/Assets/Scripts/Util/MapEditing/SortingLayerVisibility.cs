using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.Util.MapEditing
{
    /// <summary>
    /// Class managing the visibility state of sorting layers.
    /// </summary>
    [System.Serializable]
    public class SortingLayerVisibility
    {
        // Serializable list of the layers state
        [SerializeField]
        List<bool> layerStates_ = new List<bool>();
        
        /// <summary>
        /// Should be called from a MonoBehaviour Awake, must use this for initialization since SortingLayers can only be accessed
        /// from the unity main thread and unity will call non-monobehaviour constructors constantly 
        /// as a part of the serialization process.
        /// </summary>
        public void Awake()
        {
            int layerCount = SortingLayer.layers.Length;
            if( layerStates_.Count != layerCount )
                for (int i = 0; i < SortingLayer.layers.Length; ++i)
                    layerStates_.Add(true);
        }

        public void Set( SortingLayer layer, bool isVisible )
        {
            int i = SortingLayerUtil.GetLayerIndex(layer);
            layerStates_[i] = isVisible;
        }

        public bool Get( SortingLayer layer )
        {
            int i = SortingLayerUtil.GetLayerIndex(layer);
            return layerStates_[i];
        }
    }
}
