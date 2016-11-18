using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;

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
    
    /// <summary>
    /// A map consisting of chunks of tiles. The map will add and manage chunks as needed as tiles are painted in.
    /// The chunks themselves (or some component of them) are responsible for rendering the tiles.
    /// The map has no actual "borders", tiles should be able to be painted anywhere in the world.
    /// </summary>
    public class Map : MonoBehaviour
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
        /// Dictionary mapping chunks to their SortingLayer.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        ChunksToIntDict layersDict_ = new ChunksToIntDict();

        // This coudl be useful for things like hiding all tiles at a certain height.
        /// <summary>
        /// Dictionary mapping chunks to their height.
        /// </summary>
        //[SerializeField]
        //[HideInInspector]
        //ChunksToIntDict heightDict_ = new ChunksToIntDict();

        public Tile tile_;

        public IntVector3 pos_;
  
        void Awake()
        {
        }

        /// <summary>
        /// Retrieve the list of tiles at the given position, where each index of the list represents a SortingLayer.Value.
        /// The lists will be pre-populated to match the size of SortingLayer.layers.Length.
        /// Note that raw SortingLayer.Value can start below zero, see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        public List<Tile> GetTiles(IntVector3 pos)
        {
            var chunk = GetChunkWorld(pos);
            if( chunk == null )
                return null;

            return chunk.GetTilesWorld((IntVector2)pos);
        }

        /// <summary>
        /// Sets the given tile in the map at the given index. NYI : Should be able to set null to "erase" a cell (AKA set the alpha of the cell to 0)
        /// </summary>
        public void SetTile(TileIndex tIndex, Tile t)
        {
            var chunkIndex = GetChunkIndex(tIndex.Position_);
            var chunk = GetChunk(chunkIndex);

            if (chunk == null)
            {
                chunk = MakeChunk(chunkIndex, tIndex.Layer_ );
            }

            chunk.SetTileWorld(tIndex, t);
        }

        /// <summary>
        /// Sets the given tile in the map at the given position. NYI : Should be able to set null to "erase" a cell (AKA set the alpha of the cell to 0)
        /// </summary>
        public void SetTile( IntVector3 pos, Tile t )
        {
            var index = new TileIndex(pos, t.SortingLayer_);
            SetTile(index, t);
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

        /// <summary>
        /// Create a chunk at the given position in the given sorting layer. The chunks name will match the given index.
        /// </summary>
        Chunk MakeChunk( IntVector3 chunkIndex, SortingLayer layer )
        {
            var go = new GameObject(chunkIndex.ToString());
            go.transform.SetParent(transform);
            IntVector2 chunkXY = IntVector2.Scale((IntVector2)chunkIndex, chunkSize_);
            go.transform.localPosition = new Vector3(chunkXY.x, chunkXY.y, chunkIndex.z);

            var chunk = go.AddComponent<Chunk>();

            // First add the chunk to the position dict.
            chunkDict_[chunkIndex] = chunk;

            // Then to the layer dict.
            ChunkToIntDict layerChunks;
            if( !layersDict_.TryGetValue(layer.value, out layerChunks) )
            {
                layersDict_[layer.value] = layerChunks = new ChunkToIntDict();
            }

            layerChunks[layer.value] = chunk;

            return chunk;
        }

        void OnGUI()
        {
            var pos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
            GUILayout.Label("MousePos: " + pos.ToString());
            GUILayout.Label("ChunkIndex: " + GetChunkIndex((IntVector3)pos).ToString());

            if (GUILayout.Button("AddTile"))
            {
                SetTile(pos_, tile_);
            }

            if( GUILayout.Button("Print Tiles"))
            {
                foreach (var pair in chunkDict_)
                {
                    var chunk = pair.Value;

                    chunk.Print();
                }
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

        // Subclasses so dictionaries are serializable.
        [System.Serializable]
        class ChunkToPosDict : SerializableDictionary<IntVector3, Chunk> { }
        [System.Serializable]
        class ChunksToPosDict : SerializableDictionary<IntVector3, List<Chunk>> { }
        [System.Serializable]
        class ChunkToIntDict : SerializableDictionary<int, Chunk> { }
        [System.Serializable]
        class ChunksToIntDict : SerializableDictionary<int, ChunkToIntDict> { }
    }


}