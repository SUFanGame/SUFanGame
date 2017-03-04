using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SUGame.World
{
    /// <summary>
    /// Along with it's 2D position the tile layer defines the tile index for
    /// a tile within a chunk.
    /// </summary>
    public enum TileLayer
    {
        Default         = 0,
        Water           = 1,
        WaterOverlay    = 2,
        Ground          = 3,
        GroundOverlay   = 4,
        Main            = 5,
        Character       = 6,
        Foreground      = 7,
    }
}
