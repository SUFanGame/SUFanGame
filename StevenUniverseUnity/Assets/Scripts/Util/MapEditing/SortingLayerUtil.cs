using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.Util.MapEditing
{
    public class SortingLayerUtil
    {
        // Note that SortingLayer.Value can be below zero if the default sorting layer is above any user created
        // layers. We can use the "LayerIndex" functions to account for this.
        /// <summary>
        /// Returns the zero based index of the given sorting layer.
        /// </summary>
        public static int GetLayerIndex(SortingLayer layer)
        {
            return GetLayerIndex(layer.value);
        }

        /// <summary>
        /// Returns the zero-based index of the given sorting layer value.
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
    }

}