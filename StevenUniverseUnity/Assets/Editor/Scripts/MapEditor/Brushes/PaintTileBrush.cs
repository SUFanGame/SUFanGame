using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using UnityEditor;
using StevenUniverse.FanGameEditor.CustomGUI;
using System.Linq;
using StevenUniverse.FanGameEditor.SceneEditing.Util;

namespace StevenUniverse.FanGameEditor.SceneEditing.Brushes
{
    public class PaintTileBrush : MapEditorBrush
    {
        public override string Name_ { get { return "Paint Tile"; } }

        protected override string IconTextureName_ { get { return "pencil"; } }

        const string PREFS_PAINTBRUSHSIZE_NAME = "MEPaintBrushSize";
        const string PREFS_SELECTEDSPRITE_NAME = "MEPaintSpriteIndex";

        int selectedSprite_ = 0;

        Tile[] tiles_ = null;
        List<Sprite> sprites_ = null;

        Vector2 scrollPos_;

        PaintMode paintMode_ = PaintMode.SPECIFIC;

        public bool shiftBeingHeld_;

        public PaintTileBrush( Tile[] tiles ) : base()
        {
            Size_ = EditorPrefs.GetInt(PREFS_PAINTBRUSHSIZE_NAME, 0);
            selectedSprite_ = EditorPrefs.GetInt(PREFS_SELECTEDSPRITE_NAME, 0);
            tiles_ = tiles;
            sprites_ = tiles.Select(t => t.Sprite_).ToList();
        }

        public override void OnMouseDown(Map map, IntVector3 pos)
        {

            PaintTiles(map, pos);
        }

        void PaintTiles( Map map, IntVector3 pos)
        {
            var tile = tiles_[selectedSprite_];
            var layer = tile.DefaultSortingLayer_;

            int undoIndex = 0;
            if( !shiftBeingHeld_ )
            {
                Undo.SetCurrentGroupName("Paint Tiles Single");
                undoIndex = Undo.GetCurrentGroup();
            }

            // Retrieve our target height based on our paint mode
            pos = paintMode_.GetHeightModePosition(pos, map);

            //Debug.Log("Position in brush:" + pos);

            foreach (var p in cursorPoints_)
            {
                var areaPos = (IntVector3)p + pos;
                var chunk = map.GetChunkWorld(areaPos);
                if (chunk == null)
                    chunk = map.MakeChunk(areaPos);
                Undo.RecordObject(chunk, "Set tiles");
                var mesh = chunk.GetLayerMesh(layer);
                Undo.RecordObject(mesh, "Set UVS");

                chunk.SetTileWorld((IntVector2)areaPos, tile);
            }

            if( !shiftBeingHeld_ )
            {
                Undo.CollapseUndoOperations(undoIndex);
                Undo.IncrementCurrentGroup();
            }

            EditorUtility.SetDirty(map.gameObject);
        }


        public override void OnDrag(Map map, IntVector3 worldPos)
        {
            if( Event.current.shift )
            {
                PaintTiles(map, worldPos);
            }


        }

        public override void OnDisable()
        {
            base.OnDisable();

            EditorPrefs.SetInt(PREFS_PAINTBRUSHSIZE_NAME, Size_);
            EditorPrefs.SetInt(PREFS_SELECTEDSPRITE_NAME, selectedSprite_);
        }

        public override void MapEditorGUI()
        {
            base.MapEditorGUI();

            paintMode_ = (PaintMode)EditorGUILayout.EnumPopup("Paint Mode", paintMode_);

            selectedSprite_ = SelectionGrids.FromSprites(selectedSprite_, sprites_, 50, 150, ref scrollPos_);


        }

        public override void RenderCursor( Map map )
        {
            shiftBeingHeld_ = Event.current.shift;

            var oldGUIColor = GUI.color;
            var oldGizmoColor = Gizmos.color;

            var cursorPos = SceneEditorUtil.GetCursorPosition();

            var cursorColor = shiftBeingHeld_ ? Color.green : Color.white;
            Gizmos.color = cursorColor;
            var offset = Vector2.one * .5f;
            Gizmos.DrawWireCube(cursorPos + Vector3.back + (Vector3)offset, Vector3.one * Size_ * 2 + Vector3.one * .5f);




            foreach ( var p in cursorPoints_)
            {
                // Convert world space to gui space
                var bl = HandleUtility.WorldToGUIPoint(cursorPos + p);
                var tr = HandleUtility.WorldToGUIPoint(cursorPos + p + Vector3.right + Vector3.up);
                // Can't use gui functions to draw directly into the scene view.
                Handles.BeginGUI();

                var col = oldGUIColor;
                // Draw a semi-transparent image of our current tile on the cursor.
                col.a = .65f;
                GUI.color = col;
                // Vertical UVs are flipped in the scene...?
                CustomGUI.SelectionGrids.DrawSprite(
                    Rect.MinMaxRect(bl.x, bl.y, tr.x, tr.y),
                    sprites_[selectedSprite_],
                    false, true);
                GUI.color = oldGUIColor;

                Handles.EndGUI();
            }


            Gizmos.color = oldGizmoColor;

            //base.RenderCursor();
        }
    }
}
