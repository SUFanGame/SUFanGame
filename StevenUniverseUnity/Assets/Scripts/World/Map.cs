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
    [ExecuteInEditMode]
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
        [HideInInspector]
        ChunkToPosDict chunkDict_ = new ChunkToPosDict();

        /// <summary>
        /// Dictionary mapping stacks of chunks to the 2D index.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        ChunkStackToPos stackDict_ = new ChunkStackToPos();

        /// <summary>
        /// Visibility data for the sorting layers in this map.
        /// </summary>
        [SerializeField]
        SortingLayerVisibility isLayerVisible_ = null;

        /// <summary>
        /// Optional way to hide and prevent access to all tiles above a certain height. 
        /// Useful for seeing and operating in chunks below other chunks.
        /// </summary>
        [SerializeField]
        int? heightCutoff_ = null;

        [SerializeField]
        bool showDebugGUI_ = true;

        const string NULL_TILE_STR = "Calling SetTile with a null argument. If you want to erase a tile, call EraseTile";

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

            if (heightCutoff_ != null && heightCutoff_ > pos.z)
                return;

            // Do nothing if the target layer is hidden
            if ( !isLayerVisible_.Get(layer) )
                return;

            var chunkIndex = GetChunkIndex(pos);
            Chunk chunk;
            if( !chunkDict_.TryGetValue(chunkIndex, out chunk ) )
            {
                //Debug.LogFormat("Couldn't find chunk in chunkDict at {0}", chunkIndex);
                chunk = MakeChunk(chunkIndex, layer );
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
        /// Returns null if no chunks are found.
        /// </summary>
        public Chunk GetTopChunk( IntVector2 worldPos )
        {
            // First get the stack of chunks at this position ( if any )
            var chunkIndex = GetChunkIndex(worldPos);

            // Check if the there is a stack at that position:
            ChunkList stack;
            if (!stackDict_.TryGetValue(chunkIndex, out stack))
            {
                return null;
            }

            // Then get the highest active chunk.
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var chunk = stack[i];
                // Ignore disabled chunks or chunks above the height cutoff
                if ( (heightCutoff_ != null && chunk.Height_ > heightCutoff_) || !chunk.isActiveAndEnabled)
                    continue;
                
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

        public void SetHeightCutoff( int cutoff )
        {
            if( heightCutoff_ == null || cutoff != heightCutoff_ )
            {
                foreach( var pair in chunkDict_ )
                {
                    var chunk = pair.Value;
                    int chunkHeight = (int)chunk.transform.position.z;
                    if (chunkHeight > cutoff)
                        chunk.gameObject.SetActive(false);
                    else
                        chunk.gameObject.SetActive(true);
                }
            }

            heightCutoff_ = cutoff;
        }

        public void DisableHeightCutoff()
        {
            heightCutoff_ = null;
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
        

        /// <summary>
        /// Create a chunk at the given position in the given sorting layer. The chunks name will match the given index.
        /// </summary>
        Chunk MakeChunk( IntVector3 chunkIndex, SortingLayer layer )
        {
            var go = new GameObject(chunkIndex.ToString());
            go.transform.SetParent(transform, false);
            //Debug.LogFormat("Making chunk at chunkindex {0}", chunkIndex);
            IntVector2 chunkXY = IntVector2.Scale((IntVector2)chunkIndex, chunkSize_);
            go.transform.localPosition = new Vector3(chunkXY.x, chunkXY.y, chunkIndex.z);

            var chunk = go.AddComponent<Chunk>();

            //Debug.LogFormat("Adding chunk to chunkDict at index {0}", chunkIndex);
            // First add the chunk to the position dict.
            chunkDict_[chunkIndex] = chunk;

            // Then add the chunk to the stack dict.
            ChunkList chunkStack;

            // Ensure empty chunk stacks always contain null refs equal to the sorting layer count.
            if( !stackDict_.TryGetValue(chunkXY, out chunkStack))
            {
                chunkStack = new ChunkList();
                stackDict_[chunkXY] = chunkStack;
            }

            chunkStack.Add(chunk);
            
            // Sort the stack by height;
            chunkStack.Sort( (a,b)=>a.Height_.CompareTo(b.Height_) );
            
            // Set the layer visiblity values from our map values.
            foreach( var l in SortingLayer.layers )
            {
                chunk.SetLayerVisibility(l, isLayerVisible_.Get(l));
            }

            return chunk;
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

            chunkSize_ = IntVector2.Clamp(chunkSize_, 1, 20);

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

            isLayerVisible_.Set(layer, false);

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
        }

        public IEnumerator<Chunk> GetEnumerator()
        {
            foreach( var pair in chunkDict_ )
            {
                yield return pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        // Subclasses so dictionaries are serializable.
        [System.Serializable]
        class ChunkToPosDict : SerializableDictionary<IntVector3, Chunk> { }
        [System.Serializable]
        class ChunkToIntDict : SerializableDictionary<int, Chunk> { }
        [System.Serializable]
        class ChunksToIntDict : SerializableDictionary<int, ChunkToIntDict> { }

        [System.Serializable]
        class ChunkList : List<Chunk> { }

        [System.Serializable]
        class ChunkStackToPos : SerializableDictionary<IntVector2, ChunkList> { }
    }


}