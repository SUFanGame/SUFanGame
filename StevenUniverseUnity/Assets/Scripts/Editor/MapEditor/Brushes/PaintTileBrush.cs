﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.World;
using UnityEditor;
using SUGame.SUGameEditor.CustomGUI;
using System.Linq;
using SUGame.SUGameEditor.MapEditing.Util;
using SUGame.Util.Common;

namespace SUGame.SUGameEditor.MapEditing.Brushes
{
    public class PaintTileBrush : MapEditorBrush
    {
        public override string Name_ { get { return "Paint Tile"; } }
        protected override string IconTextureName_ { get { return "pencil"; } }

        const string PREFS_SELECTEDSPRITE_NAME = "MEPaintSpriteIndex";

        int selectedSprite_ = 0;

        Tile[] tiles_ = null;
        List<Sprite> sprites_ = null;

        Vector2 scrollPos_;

        PaintMode paintMode_ = PaintMode.SPECIFIC;

        public bool shiftBeingHeld_;

        BrushArea area_ = new BrushArea("MEPaintBrushSize");

        public PaintTileBrush( Tile[] tiles ) : base()
        {
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

            foreach (var p in area_.Points_ )
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

            area_.OnDisable();
            EditorPrefs.SetInt(PREFS_SELECTEDSPRITE_NAME, selectedSprite_);
        }

        public override void MapEditorGUI()
        {
            base.MapEditorGUI();

            paintMode_ = (PaintMode)EditorGUILayout.EnumPopup("Paint Mode", paintMode_);
            area_.OnGUI();

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

            area_.RenderCursor();

            foreach (var p in area_.Points_)
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

        public override void OnScroll(Map map, float scrollValue)
        {
            base.OnScroll(map, scrollValue);
            area_.OnScroll(scrollValue);
        }
    }
}