using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGameEditor.SceneEditing
{
    public class PaintCursor : MapEditorCursor
    {
        public Tile tileToPaint_ = null;

        public override void Effect(Map map, IntVector3 worldPos)
        {
            map.SetTile(worldPos, tileToPaint_);
        }
    }
}
