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
        int dragUndoIndex_ = 0;

        /// <summary>
        /// Set of points collected during a drag-erase operation. <seealso cref="OnDragExit(Map)"/>for an explanation.
        /// </summary>
        HashSet<TileIndex> dragPoints_ = new HashSet<TileIndex>();
        IntVector3 lastDragPoint = IntVector3.one * int.MinValue;
        SortingLayer? dragLayer_ = null;

        const string PREFS_ERASEBRUSHSIZE_NAME = "MEEraserBrushSize";

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

        public EraseBrush() : base()
        {
            Size_ = EditorPrefs.GetInt(PREFS_ERASEBRUSHSIZE_NAME, 0);
        }

        public override void RenderCursor( Map map )
        {
            shiftBeingHeld_ = Event.current.shift;
            if( !shiftBeingHeld_ && dragging_ )
            {
                dragging_ = false;
                OnDragExit( map );
            }

            var oldColor = Gizmos.color;
            Gizmos.color = Color.red;
            base.RenderCursor( map );
            Gizmos.color = oldColor;
        }

        public override void OnMouseDown(Map map, IntVector3 worldPos)
        {

            if (Event.current.shift)
                return;

            SortingLayer layer;
            var chunk = map.GetChunkWorld(worldPos);
            if (chunk == null)
                return;
            var tile = chunk.GetTopTileWorld((IntVector2)worldPos, out layer);
            if ( tile == null )
            {
                Debug.Log("You must select a starting tile to determine which layer to erase");
                return;
            }

            EraseTiles(map, worldPos, layer);
        }

        public override void OnDrag(Map map, IntVector3 worldPos)
        {
            base.OnDrag(map, worldPos);
            
            // We're doing some expensive stuff here so avoid spamming it if possible.
            // Don't count drags on the same grid cell and ignore drags if shift isn't being held.
            if(!Event.current.shift )
            {
                return;
            }

            // If we haven't started dragging yet we'll try to pick our "Current layer", which will be the one of the tile under the center cursor.
            if (dragging_ == false)
            {
                // This is where our drag operation starts.

                // Set up our undo group so we can undo all tile/mesh operations at once
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("Drag Erase");
                dragUndoIndex_ = Undo.GetCurrentGroup();
                // Clear our points used to track our drag area.
                dragPoints_.Clear();
                dragging_ = true;
            }

            if ( dragging_ )
            {
                // The drag layer defines which layer all cells of the erase cursor will target. Drag operations will be ignored
                // until the CENTER cursor is dragged on a valid tile.
                if( dragLayer_ == null )
                {
                    var chunk = map.GetTopChunk((IntVector2)worldPos);
                    if (chunk == null)
                    {
                        DragWarning();
                        return;
                    }
                    SortingLayer layer;
                    var tile = chunk.GetTopTileWorld((IntVector2)worldPos, out layer);
                    if (tile == null)
                    {
                        DragWarning();
                        return;
                    }
                    //Debug.LogFormat("Setting drag layer to {0}", layer.name);
                    dragLayer_ = layer;
                }

                if( lastDragPoint == worldPos )
                {
                    return;
                }
                lastDragPoint = worldPos;
                
                // Iterate over all our cursor points and set the target meshes to null.
                foreach (var p in cursorPoints_)
                {
                    var areaPos = (IntVector2)worldPos + p;

                    var chunk = map.GetTopChunk(areaPos);
                    if (chunk == null)
                        continue;

                    //Debug.LogFormat("Polling chunk for tile at {0}", areaPos);
                    // Get the topmost tile
                    var tile = chunk.GetTileWorld(areaPos, dragLayer_.Value);

                    //Debug.LogFormat("Found tile at {0}", areaPos);
                    if (tile != null)
                    {
                        // Add the point to our drag points. These will be used to erase all tiles at once since
                        // doing one by one seems to not work with Undo. See OnDragExit for an explanation.
                        dragPoints_.Add(new TileIndex(areaPos, dragLayer_.Value));

                        // We still want to give immediate user feedback so set the mesh colors immediately, see OnDragExit for an explanation.
                        var mesh = chunk.GetLayerMesh(dragLayer_.Value);
                        var localPos = areaPos - (IntVector2)chunk.transform.position;
                        Undo.RecordObject(mesh, "Hide colors");
                        mesh.SetColors(localPos, default(Color32));
                        mesh.ImmediateUpdate();
                    }
                }
            }

        }

        void DragWarning()
        {
            Debug.Log("Drag erases won't start until the center of your cursor is over a valid tile.");
        }

        public override void OnMouseUp(Map map, IntVector3 worldPos)
        {
            base.OnMouseUp(map, worldPos);

            if (dragging_)
            {
                dragging_ = false;
                OnDragExit(map);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            EditorPrefs.SetInt(PREFS_ERASEBRUSHSIZE_NAME, Size_);
        }

        /// <summary>
        /// Called when a drag ends, either by releasing the shift key or raising the mouse button.
        /// At this point the meshes have already been erased where we were dragging. Now we want to 
        /// do one big operation to erase the tile data.
        /// 
        /// Previously I tried to erase tiles constantly during dragging, but Unity's Undo system didn't like that,
        /// and when you tried to Undo drag erases it would restore the meshes properly but not the tile references.
        /// 
        /// Erasing the tile data all at once seems to solve the issue.
        /// </summary>
        /// <param name="map"></param>
        void OnDragExit(Map map)
        {
            foreach( var index in dragPoints_ )
            {
                var chunk = map.GetTopChunk(index.position_);
                Undo.RecordObject(chunk, "Erase tile");
                chunk.EraseTileWorld(index.position_, index.Layer_);
            }
            Undo.CollapseUndoOperations(dragUndoIndex_);
            lastDragPoint = IntVector3.one * int.MinValue;
            dragLayer_ = null;
        }
        

        void EraseTiles(Map map, IntVector3 pos, SortingLayer layer )
        {
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
                    Debug.LogFormat("No chunk found at {0}", areaPos);
                    continue;
                }
                Undo.RecordObject(chunk, "Erase tiles");
                var mesh = chunk.GetLayerMesh(layer);
                //Debug.LogFormat("Setting mesh values on mesh {0} Layer {1}", mesh.name, layer.name);
                Undo.RecordObject(mesh, "Erase UVS");

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

        void SetTileTransparent( TiledMesh mesh, IntVector2 pos, SortingLayer layer )
        {

        }
    }
}
