using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SUGame.Util.MapEditing
{
    public class SortingLayerUtil
    {
        // Note that SortingLayer.Value can be below zero if the default sorting layer is above any user created
        // layers. We can use the "LayerIndex" functions to get the actual "array position" of the layers.
        /// <summary>
        /// Returns the zero based index of the given sorting layer. This is the equivalent position of the layer
        /// in the Sorting Layers list of the project.
        /// </summary>
        public static int GetLayerIndex(SortingLayer layer)
        {
            return GetLayerIndex(layer.value);
        }

        /// <summary>
        /// Returns the zero-based index of the given sorting layer value. This is the equivalent position of the layer
        /// in the Sorting Layers list of the project.
        /// </summary>
        public static int GetLayerIndex( int sortingLayerValue )
        {
            return sortingLayerValue - SortingLayer.layers[0].value;
        }

        /// <summary>
        /// Returns the total count of Sorting Layers in the project.
        /// </summary>
        public static int LayerCount
        {
            get
            {
                return SortingLayer.layers.Length;
            }
        }

        /// <summary>
        /// Returns the Sorting Layer from a sorting layer value.
        /// </summary>
        public static SortingLayer GetLayerFromValue( int sortingLayerValue )
        {
            return SortingLayer.layers[ GetLayerIndex(sortingLayerValue) ];
        }

        /// <summary>
        /// Returns a sorting layer given a sorting layer id.
        /// </summary>
        public static SortingLayer GetLayerFromID( int layerID )
        {
            int value = SortingLayer.GetLayerValueFromID(layerID);
            return GetLayerFromValue(value);
        }

        /// <summary>
        /// Returns a sorting layer given a layer's zero-based index. The index is the equivalent position of the layer
        /// in the Sorting Layers list of the project.
        /// </summary>
        public static SortingLayer GetLayerFromIndex( int index )
        {
            return SortingLayer.layers[index];
        }
    }

}