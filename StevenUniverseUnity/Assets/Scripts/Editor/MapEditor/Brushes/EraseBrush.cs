using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util;
using SUGame.World;
using UnityEditor;
using System.Linq;
using SUGame.Util.Common;
using SUGame.SUGameEditor.MapEditing.Util;
using SUGame.World.DynamicMesh;

// TODO : Should be able to cut this down a bit, combine the single click and drag functions a bit more.
// Note this is unfortunately pretty complicated. Unity's undo system has a lot of quirks that needed to be worked around.
//    Specifically I ran into a lot of issues Undo-ing drag-erase operations. See OnDragExit for details.
namespace SUGame.SUGameEditor.MapEditing.Brushes
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
        //SortingLayer? dragLayer_ = null;
        TileLayer? dragLayer_ = null;
        IntVector3 dragOriginPoint_;

        BrushArea area_ = new BrushArea(PREFS_ERASEBRUSHSIZE_NAME);

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
            //var cursorPos = SceneEditorUtil.GetCursorPosition();

            //var offset = Vector2.one * .5f;
            ////Gizmos.DrawWireCube(cursorPos + (Vector3)offset + Vector3.back * 10, Vector3.one * (area_.Size_ * 2 + 1));
            //foreach( var p in area_.Points_)
            //{
            //    Gizmos.DrawWireCube(cursorPos + (Vector3)offset + Vector3.back  + p, Vector3.one *.95f );
            //}
            area_.RenderCursor();

            //base.RenderCursor( map );
            Gizmos.color = oldColor;
        }

        public override void OnMouseDown(Map map, IntVector3 worldPos)
        {

            if (Event.current.shift)
                return;

            //Get the layer and adjusted worldPos of the click based on the burhs settings and the click location
            TileLayer layer;
            worldPos = GetTargetPosition(map, worldPos, out layer);

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
                    TileLayer layer;
                    var tile = chunk.GetTopTileWorld((IntVector2)worldPos, out layer);
                    if (tile == null)
                    {
                        DragWarning();
                        return;
                    }
                    //Debug.LogFormat("Setting drag layer to {0}", layer.name);
                    // Keep track of the position and layer that the actual drag erase starts.
                    // The drag erase will ONLY erase tiles that are on this same height and layer.
                    dragLayer_ = layer;

                    dragOriginPoint_ = worldPos;
                    dragOriginPoint_.z = chunk.Height_;
                }

                worldPos.z = dragOriginPoint_.z;
                
                // Iterate over all our cursor points and set the target meshes to null.
                foreach (var p in area_.Points_)
                {
                    var areaPos = worldPos + (IntVector3)p;

                    // Only target chunks on the same height as the tile that started our drag erase.
                    var chunk = map.GetChunkWorld(areaPos);
                    if (chunk == null)
                        continue;

                    //Debug.LogFormat("Polling chunk for tile at {0}", areaPos);
                    // Get the topmost tile
                    // Only target tiles on the same layer as the tile that started our drag erase.
                    var tile = chunk.GetTileWorld((IntVector2)areaPos, dragLayer_.Value);

                    //Debug.LogFormat("Found tile at {0}", areaPos);
                    if (tile != null)
                    {
                        // Add the point to our drag points. These will be used to erase all tiles at once since
                        // doing one by one seems to not work with Undo. See OnDragExit for an explanation.
                        dragPoints_.Add(new TileIndex((IntVector2)areaPos, dragLayer_.Value));

                        // We still want to give immediate user feedback so set the mesh colors immediately, see OnDragExit for an explanation.
                        var mesh = chunk.GetLayerMesh(dragLayer_.Value);
                        var localPos = (IntVector2)areaPos - (IntVector2)chunk.transform.position;
                        Undo.RecordObject(mesh, "Hide colors");
                        mesh.SetHidden(localPos);
                        mesh.ImmediateUpdate();
                    }
                }
            }

        }

        public override void MapEditorGUI()
        {
            base.MapEditorGUI();

            area_.OnGUI();
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
            area_.OnDisable();
        }

        public override void OnScroll(Map map, float scrollValue)
        {
            base.OnScroll(map, scrollValue);

            area_.OnScroll(scrollValue);
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
            float progress = 0;
            foreach( var index in dragPoints_ )
            {
                var chunk = map.GetTopChunk(index.position_);
                Undo.RecordObject(chunk, "Erase tile");
                chunk.EraseTileWorld(index.position_, index.Layer_);
                ++progress;
                EditorUtility.DisplayProgressBar("Drag Erase", "Completing Drag Erase Operation", progress / (float)(dragPoints_.Count - 1));
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(dragUndoIndex_);
            dragLayer_ = null;
            EditorUtility.SetDirty(map.gameObject);
        }
        

        void EraseTiles(Map map, IntVector3 worldPos, TileLayer layer )
        {
            int undoIndex = 0;
            if (!shiftBeingHeld_)
            {
                Undo.SetCurrentGroupName("Erase Tiles Single");
                undoIndex = Undo.GetCurrentGroup();
            }

            //Debug.LogFormat("CursorPoints: {0}", string.Join(",", cursorPoints_.Select(p => p.ToString()).ToArray()));

            foreach (var p in area_.Points_)
            {
                IntVector3 offset = new IntVector3(p.x, p.y, 0);
                IntVector3 areaPos = worldPos + offset;
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
        public override IntVector3 GetTargetPosition(Map map, IntVector3 worldPos)
        {
            TileLayer targetLayer;
            return GetTargetPosition(map, worldPos, out targetLayer);
        }

        public override IntVector3 GetTargetPosition(Map map, IntVector3 worldPos, out TileLayer targetLayer)
        {
            //targetLayer = SortingLayer.layers[0];
            targetLayer = TileLayer.Default;

            switch (brushMode_)
            {
                case BrushMode.TOP:
                    {
                        var chunk = map.GetTopChunk((IntVector2)worldPos);
                        if (chunk == null)
                            break;

                        //Set the target layer to the layer of the top tile
                        chunk.GetTopTileWorld((IntVector2)worldPos, out targetLayer);

                        worldPos.z = chunk.Height_;
                    }
                    break;
                case BrushMode.SPECIFIC:
                    {
                        //TODO allow the user to choose a specific layer to erase as well (only show this drop-down when the "specific" BrushMode is selected)
                        targetLayer = TileLayer.Default;

                        worldPos.z = MapEditor.SpecificCursorHeight_;
                    }
                    break;
            }

            return worldPos;
        }

        /// <summary>
        /// Defines the index of a tile within a chunk. Used to batch erase operations
        /// in a list.
        /// </summary>
        [System.Serializable]
        public struct TileIndex : System.IEquatable<TileIndex>
        {
            public IntVector2 position_;

            [SerializeField]
            TileLayer layer_;

            public TileLayer Layer_ { get { return layer_; } }

            public TileIndex(IntVector2 pos, TileLayer layer)
            {
                position_ = pos;
                layer_ = layer;
            }

            public TileIndex(int x, int y, TileLayer layer) : this(new IntVector2(x, y), layer)
            { }

            public override bool Equals(object obj)
            {
                if (!(obj is TileIndex))
                    return false;

                TileIndex p = (TileIndex)obj;

                return this.Equals(p);
            }

            public bool Equals(TileIndex other)
            {
                return position_ == other.position_ && layer_ == other.layer_;
            }

            public static bool operator ==(TileIndex lhs, TileIndex rhs)
            {
                return lhs.Equals(rhs);
            }

            public static bool operator !=(TileIndex lhs, TileIndex rhs)
            {
                return !lhs.Equals(rhs);
            }

            public static TileIndex operator -(TileIndex lhs, TileIndex rhs)
            {
                return new TileIndex(lhs.position_ - rhs.position_, lhs.Layer_);
            }

            //http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;

                    hash = hash * 23 + position_.GetHashCode();
                    hash = hash * 23 + layer_.GetHashCode();
                    return hash;
                }
            }

            public override string ToString()
            {
                return string.Format("[{0},{1}]", position_.ToString("0"), layer_.ToString());
            }

        }
    }
}
