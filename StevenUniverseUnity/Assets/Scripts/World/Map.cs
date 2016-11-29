using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.MapEditing;
using System.Linq;

namespace StevenUniverse.FanGame.World
{
    // For animated cells: These will be separated into a separate mesh built for this purpose?
    // FOr instance : If you have a layer consisting of a bunch of different types of non animated tiles, it may still be one mesh.
    // If you then try to paint an animating water tile on top of one of those non-animating tiles, it would have to:
    // Remove the existing cell from the non-animating mesh
    // Create a new animating mesh and build a cell at that position
    // Populate the cell on the animating mesh with the animating tile.
    // ^^ This method seems awkward. Is there a better solution?

    // TODO : Import Dust's rules for how tiles should be placed.
    // Keep in mind the features we'll need for map editing. Things like modifying entire layers of tiles, flood fill, cursor resizing, Undo, etc

    // TODO : Layers can be hidden or shown via the map editor. The chunk should ignore ANY operations attempted
    // on a "hidden" layer. The Map class will follow these rules as well.
    // Layers will be hidden visually as well. There's a few options - since Meshes are already divided by sorting layers it's probably
    // easiest to just iterate through all chunks and hide/show the matching layer for each chunk.
    // Could also use Unity's camera culling: http://answers.unity3d.com/questions/561274/using-layers-to-showhide-different-players.html
    // Note in both the latter examples this would require setting up Layers that match the SortingLayers and having the meshes be set to the
    // appropriate Layer.

    /// <summary>
    /// A map consisting of chunks of tiles. The map will add and manage chunks as needed as tiles are painted in.
    /// The chunks themselves (or some component of them) are responsible for rendering the tiles.
    /// The map has no actual "borders", tiles should be able to be painted anywhere in the world.
    /// </summary>
    [ExecuteInEditMode, SelectionBase]
    public class Map : MonoBehaviour, IEnumerable<Chunk>
    {
        [SerializeField]
        IntVector2 chunkSize_;
        public IntVector2 ChunkSize_
        {
            get { return chunkSize_; }
            set { chunkSize_ = value; }
        }
        [SerializeField,HideInInspector]
        IntVector2 lastChunkSize_;

        /// <summary>
        /// Dictionary mapping chunks to their 3D index (convert world to index via <seealso cref="GetChunkIndex(IntVector3)"/>.
        /// </summary>
        [SerializeField]
        //[HideInInspector]
        ChunkToPosDict chunkDict_ = new ChunkToPosDict();

        /// <summary>
        /// Dictionary mapping stacks of chunks to their 2D index.
        /// </summary>
        [SerializeField]
        //[HideInInspector]
        ChunkStackToPosDict stackDict_ = new ChunkStackToPosDict();

        /// <summary>
        /// Just a big'ol list o' chunks.
        /// </summary>
        [SerializeField]
        List<Chunk> chunks_ = new List<Chunk>();

        /// <summary>
        /// Visibility data for the sorting layers in this map.
        /// </summary>
        [SerializeField]
        SortingLayerVisibility isLayerVisible_ = null;

        /// <summary>
        /// Optional way to hide and prevent access to all tiles above a certain height. 
        /// Useful for seeing and operating in chunks below other chunks.
        /// </summary>

        public CutoffType cutoffType_ = CutoffType.NONE;
        public int heightCutoff_ = 0;

        /// <summary>
        /// Inside the tiled mesh we rely on the alpha of cells to define an empty cell from within that class. Thus we can't
        /// explicitly allow the user to set all tiles in the mesh to alpha of 0, since we then have no way of knowing ( from within the tiled mesh class,
        /// ) which tiles are hidden and what aren't.
        /// 
        /// To that end we will track when meshes are being "hiddeN" and instead hide the renderers.
        /// </summary>
        [SerializeField,HideInInspector]
        bool cutoffChunksHidden_ = false;

        [SerializeField]
        bool showDebugGUI_ = true;
         
        const string NULL_TILE_STR = "Calling SetTile with a null argument. If you want to erase a tile, call EraseTile";
        


        [SerializeField]
        Material terrainMaterial_;

        void Awake()
        {
            isLayerVisible_ = new SortingLayerVisibility();
            isLayerVisible_.Awake();
        }

        /// <summary>
        /// Sets the given tile in the map at the given position and layer. Will create a chunk at the given position
        /// if one doesn't exist.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="layer"></param>
        /// <param name="t"></param>
        public void SetTile( IntVector3 pos, SortingLayer layer, Tile t )
        {
            if (t == null)
            {
                Debug.LogError(NULL_TILE_STR);
                return;
            }

            if (cutoffType_ != CutoffType.NONE && cutoffType_.IsCutoff(heightCutoff_, pos.z) )
                return;

            // Do nothing if the target layer is hidden
            if ( !isLayerVisible_.Get(layer) )
                return;

            var chunkIndex = GetChunkIndex(pos);
            Chunk chunk;
            if( !chunkDict_.TryGetValue(chunkIndex, out chunk ) )
            {
                //Debug.LogFormat("Couldn't find chunk in chunkDict at {0}", chunkIndex);
                chunk = MakeChunkFromIndex(chunkIndex );
            }
            else if( !chunk.isActiveAndEnabled )
            {
                Debug.LogWarningFormat("Attempting to set tile data on a disabled chunk at {0}", pos);
            }

            chunk.SetTileWorld(new TileIndex((IntVector2)pos, layer), t);
        }
        
        /// <summary>
        /// Paints a tile on the topmost visible chunk at the given position.
        /// If no chunks are visible, one will be created at height 0.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="t"></param>
        public void SetTile(IntVector2 pos, Tile t)
        {
            var topChunk = GetTopChunk(pos);
            
            if (topChunk == null)
                SetTile(new IntVector3(pos.x, pos.y, 0), t.DefaultSortingLayer_, t);
            else
            {
                SetTile(new IntVector3(pos.x, pos.y, topChunk.Height_), t.DefaultSortingLayer_, t);
            }
        }
        
        /// <summary>
        /// Sets the given tile in the map at the given position. The tile's default layer will be used.
        /// </summary>
        public void SetTile(IntVector3 pos, Tile t)
        {
            SetTile(pos, t.DefaultSortingLayer_, t);
        }


        /// <summary>
        /// Erase the topmost visible tile at the given position.
        /// </summary>
        /// <param name="pos"></param>
        public void EraseTile(IntVector2 pos)
        {
            var topChunk = GetTopChunk(pos);

            if (topChunk == null)
                return;

            topChunk.EraseTileWorld(pos);
        }

        /// <summary>
        /// Retrieve the topmost visible chunk at the given position.
        /// This will ignore chunks that have no tiles at the given position.
        /// Returns null if no chunks are found.
        /// </summary>
        public Chunk GetTopChunk( IntVector2 worldPos )
        {
            // First get the stack of chunks at this position ( if any )
            var chunkIndex = GetChunkIndex(worldPos);

            // Check if the there is a stack at that position:
            ChunkListWrapper wrapper;
            if (!stackDict_.TryGetValue(chunkIndex, out wrapper))
            {
                return null;
            }
            var stack = wrapper.value_;

            //Debug.LogFormat("Stack at chunk index {0}: {1}", chunkIndex, stack.Count);

            // Then get the highest active chunk.
            for (int i = 0; i < stack.Count; ++i)
            {
                var chunk = stack[i];

                //Debug.LogFormat("Checking chunk {0}", chunk.name);
                // Ignore disabled chunks or chunks above the height cutoff
                if ( cutoffType_ != CutoffType.NONE && cutoffType_.IsCutoff( heightCutoff_, chunk.Height_) || !chunk.isActiveAndEnabled)
                    continue;

                SortingLayer layer;
                if (chunk.GetTopTileWorld(worldPos, out layer) != null)
                    return chunk;

            }

            return null;
        }

        /// <summary>
        /// Retrieve a chunk from a world position.
        /// </summary>
        public Chunk GetChunkWorld( IntVector3 worldPos )
        {
            var chunkIndex = GetChunkIndex(worldPos);

            return GetChunk(chunkIndex);
        }


        /// <summary>
        /// Retrieve a chunk from a ChunkIndex.
        /// </summary>
        Chunk GetChunk( IntVector3 chunkIndex )
        {
            Chunk chunk;
            chunkDict_.TryGetValue(chunkIndex, out chunk);
            return chunk;
        }

        /// <summary>
        /// Get the 3D chunk index from the given world position.
        /// </summary>
        IntVector3 GetChunkIndex(IntVector3 pos)
        {
            pos.x = Mathf.FloorToInt((float)pos.x / (float)chunkSize_.x);
            pos.y = Mathf.FloorToInt((float)pos.y / (float)chunkSize_.y);

            return pos;
        }

        IntVector2 GetChunkIndex( IntVector2 pos )
        {
            pos.x = Mathf.FloorToInt((float)pos.x / (float)chunkSize_.x);
            pos.y = Mathf.FloorToInt((float)pos.y / (float)chunkSize_.y);

            return pos;
        }

        public Chunk MakeChunk(IntVector3 worldPos)
        {
            return MakeChunkFromIndex(GetChunkIndex(worldPos));
        }

        /// <summary>
        /// Create a chunk at the given position in the given sorting layer. The chunks name will match the given index.
        /// </summary>
        Chunk MakeChunkFromIndex( IntVector3 chunkIndex )
        {
            var go = new GameObject(chunkIndex.ToString());
            go.transform.SetParent(transform, false);
            //Debug.LogFormat("Making chunk at chunkindex {0}", chunkIndex);
            IntVector2 chunkXY = IntVector2.Scale((IntVector2)chunkIndex, chunkSize_);
            go.transform.localPosition = new Vector3(chunkXY.x, chunkXY.y, chunkIndex.z);

            var chunk = go.AddComponent<Chunk>();
            chunk.Size_ = chunkSize_;

            chunks_.Add(chunk);

            //Debug.LogFormat("Adding chunk to chunkDict at index {0}", chunkIndex);
            // First add the chunk to the position dict.
            chunkDict_[chunkIndex] = chunk;

            // Then add the chunk to the stack dict.
            ChunkListWrapper wrapper;

            // Ensure empty chunk stacks always contain null refs equal to the sorting layer count.
            if (!stackDict_.TryGetValue((IntVector2)chunkIndex, out wrapper))
            {
                wrapper = new ChunkListWrapper();
                stackDict_[(IntVector2)chunkIndex] = wrapper;
            }

            var chunkStack = wrapper.value_;

            chunkStack.Add(chunk);

            // Sort the stack by highest-first;
            chunkStack.Sort( (a, b) => -a.Height_.CompareTo(b.Height_));

            // Set the layer visiblity values from our map values.
            foreach (var l in SortingLayer.layers)
            {
                chunk.SetLayerVisibility(l, isLayerVisible_.Get(l));
            }


            chunk.terrainMaterial_ = terrainMaterial_;

            

            return chunk;
        }

        /// <summary>
        /// Get the mesh at the given position and layer.
        /// </summary>
        public TiledMesh GetMesh( IntVector3 pos, SortingLayer layer )
        {
            if (cutoffType_ != CutoffType.NONE && cutoffType_.IsCutoff(heightCutoff_, pos.z) )
            {
                Debug.LogError("Attempting to retrieve a mesh from above the height cutoff.");
                return null;
            }

            // Do nothing if the target layer is hidden
            if (!isLayerVisible_.Get(layer))
            {
                Debug.LogError("Attempting to retrieve a mesh for a hidden layer");
                return null;
            }

            var chunkIndex = GetChunkIndex(pos);
            Chunk chunk;
            if (!chunkDict_.TryGetValue(chunkIndex, out chunk))
            {
                //Debug.LogFormat("Couldn't find chunk in chunkDict at {0}", chunkIndex);
                chunk = MakeChunkFromIndex(chunkIndex);
            }
            else if (!chunk.isActiveAndEnabled)
            {
                Debug.LogWarningFormat("Attempting to retrieve a mesh from a disabled chunk at {0}", pos);
            }

            return chunk.GetLayerMesh(layer);

        }


        void OnGUI()
        {
            if (!showDebugGUI_ || !isActiveAndEnabled)
                return;

            if( Application.isPlaying )
            {
                var pos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
                GUILayout.Label("MousePos: " + pos.ToString());
                GUILayout.Label("ChunkIndex: " + GetChunkIndex((IntVector3)pos).ToString());
            }

        }

        void OnValidate()
        {
            if (!isActiveAndEnabled)
                return;

            chunkSize_ = IntVector2.Clamp(chunkSize_, 1, Chunk.MAXSIZE );

            if( chunkSize_ != lastChunkSize_ )
            {
                lastChunkSize_ = chunkSize_;

                // TODO: If chunk size is change the entire map would have to be rebuilt. Probably a way to maintain tile references in the cases
                // where the chunks are resized.
            }

        }

        /// <summary>
        /// Set the visibility state of the given layer.
        /// </summary>
        public void SetLayerVisible( SortingLayer layer, bool visible )
        {
            if( visible == false )
            {
                HideLayer(layer);
            }
            else
            {
                ShowLayer(layer);
            }
        }

        public bool GetLayerVisible( SortingLayer layer )
        {
            return isLayerVisible_.Get(layer);
        }

        /// <summary>
        /// Hide all tiles of the given layer for the entire map.
        /// </summary>
        /// <param name="layer"></param>
        public void HideLayer( SortingLayer layer )
        {
            if (!isLayerVisible_.Get(layer))
                return;

            isLayerVisible_.Set(layer, false);

            var enumerator = chunkDict_.GetEnumerator();
            while( enumerator.MoveNext())
            {
                var chunk = enumerator.Current.Value;
                chunk.HideLayer(layer);
            }
        }

        /// <summary>
        /// Show all tiles of the given layer for the entire map.
        /// </summary>
        /// <param name="layer"></param>
        public void ShowLayer( SortingLayer layer )
        {
            if (isLayerVisible_.Get(layer))
                return;

            isLayerVisible_.Set(layer, true);

            var enumerator = chunkDict_.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var chunk = enumerator.Current.Value;
                chunk.ShowLayer(layer);
            }
        }

        public void Clear()
        {
            var chunks = GetComponentsInChildren<Chunk>();
            for( int i = chunks.Length - 1; i >= 0; --i )
            {
                DestroyImmediate(chunks[i].gameObject);
            }

            isLayerVisible_.SetAll(true);
            chunkDict_.Clear();
            stackDict_.Clear();
            chunks_.Clear();
        }

        /// <summary>
        /// Sets the alpha value for the meshes on all "cut off" chunks.
        /// </summary>
        /// <param name="map"></param>
        public void SetCutoffAlphaOnChunks( byte alpha )
        {
            if( alpha == 0 )
            {
                HideCutoffChunks();
                return;
            }

            if( cutoffChunksHidden_ && alpha != 0 )
            {
                ShowCutoffChunks();
            }  

            //Debug.Log("Set cutoff alpha" );
            foreach (var chunk in this)
            {
                //Debug.Log("sETTING ALPHA TO " + alpha.ToString() + " in " + chunk.name );

                bool cutoff = cutoffType_.IsCutoff(heightCutoff_, chunk.Height_);

                if (cutoff)
                    chunk.SetVisibleAlpha(alpha);
                else
                    chunk.SetVisibleAlpha(255);
            }
        }

        
        public void HideCutoffChunks()
        {
            if( cutoffChunksHidden_ == false )
            {
                foreach( var chunk in this )
                {
                    bool cutoff = cutoffType_.IsCutoff(heightCutoff_, chunk.Height_);

                    chunk.Mesh.enabled = !cutoff;
                }
            }

            cutoffChunksHidden_ = true;
        }

        public void ShowCutoffChunks()
        {
            if( cutoffChunksHidden_ == true )
            {
                foreach (var chunk in this)
                {
                    bool cutoff = cutoffType_.IsCutoff(heightCutoff_, chunk.Height_);

                    chunk.Mesh.enabled = true;
                }
            }
            cutoffChunksHidden_ = false;
        }

        public void RefreshMesh()
        {
            var enumerator = this.GetEnumerator();
            while( enumerator.MoveNext() )
            {
                var chunk = enumerator.Current;
                chunk.RefreshMesh();
            }
        }

        public void ClearEmptyChunks()
        {
            for( int i = chunks_.Count - 1; i >= 0; --i )
            {
                var chunk = chunks_[i];
                if( chunk.IsEmpty )
                {
                    RemoveChunk(chunk.Chunkindex_);
                }
            }
        }

        public void RemoveChunk( IntVector3 chunkIndex )
        {
            var chunk = chunkDict_[chunkIndex];
            chunkDict_.Remove(chunkIndex);
            var stack = stackDict_[(IntVector2)chunkIndex].value_;
            stack.Remove(chunk);
            if( stack.Count == 0 )
            {
                stackDict_.Remove((IntVector2)chunkIndex);
            }
            chunks_.Remove(chunk);
            DestroyImmediate(chunk.gameObject);
        }

        public IEnumerator<KeyValuePair<IntVector2,List<Chunk>>> GetChunkStackEnumerator()
        {
            foreach( var pair in stackDict_ )
            {
                yield return new KeyValuePair<IntVector2, List<Chunk>>(pair.Key, pair.Value.value_);
            }
        }

        public IEnumerator<Chunk> GetEnumerator()
        {
            foreach (var pair in chunkDict_)
            {
                yield return pair.Value;
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void PrintTiles()
        {
            foreach (var chunk in this )
                chunk.Print();
        }

        // Subclasses so dictionaries are serializable.
        [System.Serializable]
        class ChunkToPosDict : SerializableDictionary<IntVector3, Chunk> { }

        [System.Serializable]
        class ChunkListWrapper { public List<Chunk> value_ = new List<Chunk>(); }

        [System.Serializable]
        class ChunkStackToPosDict : SerializableDictionary<IntVector2, ChunkListWrapper> { }
    }


}