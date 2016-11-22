using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using UnityEditor;
using StevenUniverse.FanGameEditor.CustomGUI;
using System.Linq;

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

        public PaintTileBrush( Tile[] tiles ) : base()
        {
            Size_ = EditorPrefs.GetInt(PREFS_PAINTBRUSHSIZE_NAME, 0);
            selectedSprite_ = EditorPrefs.GetInt(PREFS_SELECTEDSPRITE_NAME, 0);
            tiles_ = tiles;
            sprites_ = tiles.Select(t => t.Sprite_).ToList();
        }

        protected override void OnClick(Map map, IntVector3 worldPos )
        {
            var tile = tiles_[selectedSprite_];
            //Undo.RecordObject(map.gameObject, "SetTile");
            //var mesh = map.GetMesh(worldPos, tiles_[selectedSprite_].DefaultSortingLayer_);
            var chunk = map.GetChunkWorld(worldPos);

            if( chunk == null )
            {
                chunk = map.MakeChunk(worldPos);
            }

            var mesh = chunk.GetLayerMesh(tile.DefaultSortingLayer_);
            
            Undo.RegisterCompleteObjectUndo(chunk, "Set Tile in Chunk");
            Undo.RegisterCompleteObjectUndo(mesh, "Set mesh data");

            chunk.SetTileWorld((IntVector2)worldPos, tile);

            //mesh.ImmediateUpdate();
            //map.SetTile((IntVector2)worldPos, tiles_[selectedSprite_]);
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
            EditorPrefs.SetInt(PREFS_SELECTEDSPRITE_NAME, selectedSprite_);
        }

        public override void MapEditorGUI()
        {
            base.MapEditorGUI();

            selectedSprite_ = SelectionGrids.FromSprites(selectedSprite_, sprites_, 50, 150, ref scrollPos_);
        }
    }
}
