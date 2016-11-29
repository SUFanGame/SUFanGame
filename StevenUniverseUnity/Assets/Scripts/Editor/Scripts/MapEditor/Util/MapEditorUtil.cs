using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGameEditor.SceneEditing.Util
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

        /// <summary>
        /// Returns a 3D position on the map matching the given HeightMode.
        /// </summary>
        /// <param name="mode">The height mode to us.</param>
        /// <param name="worldPos">The 3D world position on the map. The z value represents the current height.</param>
        /// <param name="map">The map to poll.</param>
        /// <returns></returns>
        public static IntVector3 GetHeightModePosition( this PaintMode paintMode, IntVector3 worldPos, Map map )
        {

            switch( paintMode )
            {
                case PaintMode.OVERWRITE:
                    {
                        var chunk = map.GetTopChunk((IntVector2)worldPos);
                        if (chunk == null)
                            break;
                        worldPos.z = chunk.Height_;
                        //SortingLayer topLayer;
                        //var tile = chunk.GetTopTileWorld( worldPos, out topLayer );
                    }
                    break;
                case PaintMode.ADDITIVE:
                    {
                        var chunk = map.GetTopChunk((IntVector2)worldPos);
                        if (chunk == null)
                            break;
                        worldPos.z = chunk.Height_ + 1;
                    }
                    break;
                //case HeightMode.SPECIFIC:
                //    {

                //    }
                //    break;
            }


            return worldPos;
        }

        //public SortingLayer GetTargetLayer( IntVector2 pos, Map map )
        //{

        //}
    }
}