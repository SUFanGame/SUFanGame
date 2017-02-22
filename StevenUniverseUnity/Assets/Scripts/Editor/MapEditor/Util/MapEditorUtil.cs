using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.World;
using SUGame.Util.Common;

namespace SUGame.SUGameEditor.MapEditing.Util
{
    public enum PaintMode
    {
        /// <summary>
        /// Returns the highest active tile position.
        /// </summary>
        OVERWRITE = 0,
        /// <summary>
        /// Returns one above the highest active tile position.
        /// </summary>
        ADDITIVE = 1,
        /// <summary>
        /// Returns a specified height.
        /// </summary>
        SPECIFIC = 2,
    }

    //public enum LayerMode
    //{
    //    /// <summary>
    //    /// Tiles will paint onto their default layer at the given position.
    //    /// </summary>
    //    TILEDEFAULT = 0,
    //    /// <summary>
    //    /// Tiles will paint over the top most layer at the given position.
    //    /// </summary>
    //    OVERWRITE = 1,
    //    /// <summary>
    //    /// Tiles will always paint to a specific layer at the given position.
    //    /// </summary>
    //    SPECIFIC = 2,
    //}

    public static class MapEditorUtil
    {
        //HeightMode heightMode_;
        //LayerMode layerMode_;

        //public SortingLayer GetTargetLayer( IntVector2 pos, Map map )
        //{

        //}
    }
}