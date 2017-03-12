using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.World;
using SUGame.Util.Common;

namespace SUGame.SUGameEditor.MapEditing.Util
{
    public enum BrushMode
    {
        /// <summary>
        /// Returns the minimum position required to draw on top of the current top Tile at the target location.
        /// </summary>
        TOP = 0,
        /// <summary>
        /// Returns a specified height.
        /// </summary>
        SPECIFIC = 1,
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
    }
}