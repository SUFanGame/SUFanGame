using UnityEngine;
using System.Collections;
using SUGame.World;

namespace SUGame.Util.MapEditing
{
    public static class TileLayerUtil
    {
        public static int GetSortingOrder(this TileLayer target, int elevation)
        {
            return elevation * 100 + (int)target;
        }
    }
}