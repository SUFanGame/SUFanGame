using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using UnityEditor;
using System.Linq;

namespace StevenUniverse.FanGameEditor.SceneEditing.Brushes
{
    public class EraseBrush : MapEditorBrush
    {
        bool shiftBeingHeld_ = false;

        static bool dragging_ = false;
        static SortingLayer currentLayer_;
        int dragUndoIndex_ = 0;

        protected override string IconTextureName_
        {
            get
            {
                return "eraser";
            }
        }

        public override string Name_
        {
            get
            {
                return "Eraser";
            }
        }

        public override void RenderCursor()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = Color.red;
            base.RenderCursor();
            Gizmos.color = oldColor;
        }

        public override void OnMouseDown(Map map, IntVector3 pos)
        {
            if (Event.current.shift)
                return;
            //Debug.LogFormat("MOUSEDOWN");
            SortingLayer layer;
            var tile = GetTopTile(map, pos, out layer);
            if( tile == null )
            {
                Debug.Log("You must select a starting tile to determine which layer to erase");
                return;
            }

            EraseTiles(map, pos, layer);

            //map.ClearEmptyChunks();
        }

        public override void OnDrag(Map map, IntVector3 worldPos)
        {
            base.OnDrag(map, worldPos);

            // Don't allow dragging unless shift is being held.
            if( !Event.current.shift )
            {
                dragging_ = false;
                return;
            }

            // If we haven't started dragging yet we'll pick our "Current layer", which will be the one of the tile under the center cursor.
            if( dragging_ == false )
            {
                SortingLayer layer;
                var tile = GetTopTile(map, worldPos, out layer);
                if( tile == null )
                {
                    return;
                }
                else
                {
                    currentLayer_ = layer;
                    dragging_ = true;
                    Undo.SetCurrentGroupName("Drag Erase");
                    dragUndoIndex_ = Undo.GetCurrentGroup();
                }
            }

            if( dragging_ )
                EraseTiles(map, worldPos, currentLayer_);
        }

        public override void OnMouseUp(Map map, IntVector3 worldPos)
        {
            base.OnMouseUp(map, worldPos);
            if (dragging_)
            {
                dragging_ = false;
                //map.ClearEmptyChunks();
                //Undo.CollapseUndoOperations(dragUndoIndex_);
                //Undo.IncrementCurrentGroup();
            }
        }

        Tile GetTopTile( Map map, IntVector3 pos, out SortingLayer layer )
        {
            layer = default(SortingLayer);
            //Debug.LogFormat("CursorPos in entering Eraser Click : {0}", pos);
            var chunk = map.GetChunkWorld(pos);
            if (chunk == null)
            {
                Debug.LogFormat("No chunk found");
                return null;
            }
            
            return chunk.GetTopTileWorld((IntVector2)pos, out layer);
        }
        

        void EraseTiles(Map map, IntVector3 pos, SortingLayer layer )
        {
            shiftBeingHeld_ = Event.current.shift;

            int undoIndex = 0;
            if (!shiftBeingHeld_)
            {
                Undo.SetCurrentGroupName("Erase Tiles Single");
                undoIndex = Undo.GetCurrentGroup();
            }

            //Debug.LogFormat("CursorPoints: {0}", string.Join(",", cursorPoints_.Select(p => p.ToString()).ToArray()));

            foreach (var p in cursorPoints_)
            {
                var areaPos = (IntVector3)p + pos;
                var chunk = map.GetChunkWorld(areaPos);
                if (chunk == null)
                {
                    //Debug.LogFormat("No chunk found at {0}", areaPos);
                    continue;
                }
                Undo.RecordObject(chunk, "Set tiles");
                var mesh = chunk.GetLayerMesh(layer);
                Undo.RecordObject(mesh, "Set UVS");

                //Debug.LogFormat("Calling EraseTile on {0} at {1}, Layer: {2}", chunk.name, areaPos, layer.name);
                chunk.EraseTileWorld((IntVector2)areaPos, layer);
            }

            if (!shiftBeingHeld_)
            {
                Undo.CollapseUndoOperations(undoIndex);
                Undo.IncrementCurrentGroup();
            }

            EditorUtility.SetDirty(map.gameObject);
        }
    }
}
