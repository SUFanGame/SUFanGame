using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using UnityEditor;

namespace StevenUniverse.FanGameEditor.SceneEditing.Brushes
{
    public class PaintTileBrush : MapEditorBrush
    {
        public Tile tileToPaint_ = null;

        public override string Name_ { get { return "Paint Tile"; } }

        protected override string IconTextureName_ { get { return "pencil"; } }

        const string PREFS_PAINTBRUSHSIZE_NAME = "MEPaintBrushSize";

        public PaintTileBrush() : base()
        {
            Size_ = EditorPrefs.GetInt(PREFS_PAINTBRUSHSIZE_NAME, 0);
        }

        protected override void OnClick(Map map, IntVector3 worldPos )
        {
            map.SetTile(worldPos, tileToPaint_);
        }

        public override void OnScroll(Map map, float scrollValue)
        {
            base.OnScroll(map, scrollValue);

            var e = Event.current;

            if (e.shift)
                e.Use();

            if (scrollValue < 0)
            {
                Size_++;
            }
            else if (scrollValue > 0)
            {
                Size_--;
            }

            //Debug.Log("Scroll " + scrollValue);

        }

        public override void OnDisable()
        {
            base.OnDisable();

            EditorPrefs.SetInt(PREFS_PAINTBRUSHSIZE_NAME, Size_);
        }
    }
}
