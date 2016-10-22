using StevenUniverse.FanGame.Battle;
using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    public interface ITile
    {
        /// <summary>
        /// The 2D position of this tile
        /// </summary>
        IntVector2 Position { get; }
        /// <summary>
        /// The elevation of this tile
        /// </summary>
        int Elevation { get; }
        /// <summary>
        /// The layer sorting order for this tile, 
        /// determines ordering in cases where tiles share the same position and elevation.
        /// </summary>
        int SortingOrder { get; }

        string TileModeName { get; }
    }
}