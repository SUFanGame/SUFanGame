using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SUGame.Util.MapEditing
{
    /// <summary>
    /// Class managing the visibility state of sorting layers.
    /// </summary>
    [System.Serializable]
    public class SortingLayerVisibility
    {
        // Serializable list of the layers state
        [SerializeField]
        BoolList layerStates_ = new BoolList();
        
        /// <summary>
        /// Should be called from a MonoBehaviour Awake, must use this for initialization since SortingLayers can only be accessed
        /// from the unity main thread and unity will call non-monobehaviour constructors constantly 
        /// as a part of the serialization process.
        /// </summary>
        public void Awake()
        {
            int layerCount = SortingLayer.layers.Length;
            if( layerStates_.Count != layerCount )
            {
                for (int i = 0; i < SortingLayer.layers.Length; ++i)
                    layerStates_.Add(true);
            }

            //Debug.LogFormat("Layer states after awake: {0}", string.Join(",", layerStates_.Select(b => b.ToString()).ToArray() ));
        }

        public void Set( SortingLayer layer, bool isVisible )
        {
            int i = SortingLayerUtil.GetLayerIndex(layer);
            layerStates_[i] = isVisible;
        }

        public bool Get( SortingLayer layer )
        {
            //Debug.LogFormat("Layer states after in get function: {0}", string.Join(",", layerStates_.Select(b => b.ToString()).ToArray()));
            int i = SortingLayerUtil.GetLayerIndex(layer);
            return layerStates_[i];
        }

        public void SetAll( bool areVisible )
        {
            for (int i = 0; i < layerStates_.Count; ++i)
                layerStates_[i] = areVisible;
        }

        [System.Serializable]
        class BoolList : List<bool> { }
    }
}
