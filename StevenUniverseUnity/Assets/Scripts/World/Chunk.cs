using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.MapEditing;

namespace StevenUniverse.FanGame.World
{

    // TODO : Layers can be hidden or shown via the map editor. The chunk should ignore ANY operations attempted
    // on a "hidden" layer. The Map class will follow these rules as well. It may be best to have the layer states set up in
    // SortingLayerUtil, so the states can be access globally.
    // Layers will be hidden visually as well. There's a few options - since Meshes are already divided by sorting layers it's probably
    // easiest to just iterate through all chunks and hide/show the matching layer for each chunk.
    // Could also use Unity's camera culling: http://answers.unity3d.com/questions/561274/using-layers-to-showhide-different-players.html
    // Note in both the latter examples this would require setting up Layers that match the SortingLayers and having the meshes be set to the
    // appropriate Layer.

    // TODO : Get rid of all instances of IntVector3 in here, including in TileIndex? From a chunk perspective height means nothing, each
    // chunk is defined by it's height and so any incoming coordinates would only be 2D.

    /// <summary>
    /// A chunk of tiles, basically representing a small portion of one cross-section of the map ( where the cross sections are divided by height ).
    /// The map maintains a structure of chunks and creates them as they are need, as tiles are added to the map.
    /// For any 2D position of the chunk there can exist a stack of tiles: one tile for each sorting layer set up in the project.
    /// </summary>
    [RequireComponent(typeof(ChunkMesh)), ExecuteInEditMode]
    public class Chunk : MonoBehaviour, IEnumerable<KeyValuePair<TileIndex,Tile>>
    {
        /// <summary>
        /// Dictionary mapping stacks of tiles (where each index of the stack represents a sorting layer ) to their 2D position in local space.
        /// </summary>
        [SerializeField]
        TilesToIntVector2Dict sortingLayerDict_ = new TilesToIntVector2Dict();

        /// <summary>
        /// Dictionary mapping tiles directly to their tile index ( Position & SortingLayer )
        /// </summary>
        [SerializeField]
        TileToIndexDict indexDict_ = new TileToIndexDict();

        [SerializeField]
        SortingLayerVisibility isLayerVisible_ = new SortingLayerVisibility();

        ChunkMesh mesh_ = null;
        public ChunkMesh Mesh { get { return mesh_; } }

        public int Height_ { get { return (int)transform.position.z; } }

        void Awake()
        {
            //Debug.LogFormat("Setting awake on LayerVisbility in {0}", name);
            isLayerVisible_.Awake();
            
            mesh_ = GetComponent<ChunkMesh>();
        }

        /// <summary>
        /// Returns the topmost tile (in terms of sorting layers) at the given position.
        /// Returns null if no visible tiles exist at the given location.
        /// </summary>
        /// <param name="localPos">The 2D position to search, local to this chunk.</param>
        /// <param name="layer">Which layer a tile was found at, if any.</param>
        public Tile GetTopTile( IntVector2 localPos, out SortingLayer layer )
        {
            layer = default(SortingLayer);

            if (!sortingLayerDict_.ContainsKey(localPos))
                return null;

            var stack = GetTileStack(localPos);

            for( int i = stack.Count - 1; i >= 0; --i )
            {
                layer = SortingLayerUtil.GetLayerFromIndex(i);
                // Check if this layer is enabled
                if ( isLayerVisible_.Get(layer) )
                {
                    return stack[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve a single tile from the local position and sorting layer.
        /// </summary>
        public Tile GetTileLocal( IntVector2 localPosition, SortingLayer layer )
        {
            if( !isLayerVisible_.Get(layer) )
            {
                return null;
            }

            Tile t;
            indexDict_.TryGetValue(new TileIndex(localPosition, layer), out t);
            return t;
        }

        /// <summary>
        /// Retrieve a single tile from the chunk via the given world position and sorting layer.
        /// </summary>
        public Tile GetTileWorld(IntVector2 worldPos, SortingLayer layer)
        {
            if (!isLayerVisible_.Get(layer))
            {
                return null;
            }

            return GetTileLocal((IntVector2)transform.position - worldPos, layer);
        }

        /// <summary>
        /// Retrieve a list of tiles where each index of the list represents a SortingLayer.
        /// The given position should be LOCAL to the chunk. Note that raw SortingLayer.Value can start
        /// below zero, see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        private List<Tile> GetTileStack( IntVector2 localPos )
        {
            TileList tiles;
            if (!sortingLayerDict_.TryGetValue(localPos, out tiles))
            {
                // Ensure retrieved tile stacks are always populated with null refs
                sortingLayerDict_[localPos] = tiles = new TileList();
                for (int i = 0; i < SortingLayerUtil.LayerCount; ++i)
                {
                    tiles.Add(null);
                }
            }
            return tiles;
        }

        public void SetTileWorld( TileIndex index, Tile t )
        {
            //Debug.LogFormat("SetTileWorld index: {0}", index);
            if ( !isLayerVisible_.Get(index.layer_) )
            {
                //Debug.LogFormat("Layer {0} is hidden for chunk {1}", index.Layer_.name, name);
                return;
            }
            else
            {
                //Debug.LogFormat("Layer {0} is not hidden for chunk {1}", index.Layer_.name, name);
            }
            var oldPos = index.position_;
            index.position_ = index.position_ - (IntVector2)transform.localPosition;

            //Debug.LogFormat("Index after localizing: {0} - {1} : {2}", (IntVector2)transform.localPosition, oldPos, index.position_ );
            SetTileLocal(index, t);
        }

        public void SetTileWorld(IntVector2 pos, Tile t)
        {
            SetTileWorld(new TileIndex(pos, t.DefaultSortingLayer_), t);
        }

        void SetTileLocal(TileIndex index, Tile t)
        {
            //Debug.LogFormat("Setting tile for {0}", index);
            indexDict_[index] = t;
            var stack = GetTileStack(index.position_);
            stack[index.SortingLayerIndex_] = t;
            // TODO : Write to mesh

        }


        /// <summary>
        /// Erase the top most tile ( in terms of Sorting Layer ) at the given position.
        /// </summary>
        public void EraseTileWorld( IntVector2 worldPos )
        {
            EraseTileLocal( worldPos - (IntVector2)transform.localPosition );
        }

        /// <summary>
        /// Erase the top most tile at the given position.
        /// </summary>
        /// <param name="pos"></param>
        public void EraseTileLocal( IntVector2 pos )
        {
            // First check if there are any tiles here
            TileList stack;
            if( !sortingLayerDict_.TryGetValue(pos, out stack) )
            {
                return;
            }

            for( int i = stack.Count - 1; i >= 0; --i )
            {
                var t = stack[i];
                var layer = SortingLayerUtil.GetLayerFromIndex(i);
                // Ignore layers that aren't visible
                if (t == null || !isLayerVisible_.Get(layer))
                    continue;

                RemoveTile(new TileIndex(pos, layer));
                return;
            }
        }


        void RemoveTile( TileIndex index )
        {
            if (!indexDict_.ContainsKey(index))
                return;
            indexDict_.Remove(index);
            sortingLayerDict_[index.position_][index.SortingLayerIndex_] = null;
            // TODO: Remove From Mesh

        }

        public void SetLayerVisibility( SortingLayer layer, bool val )
        {
            if (val)
                ShowLayer(layer);
            else
                HideLayer(layer);
        }

        // TODO : This should hide tiles visually as well as prevent the map editor (or anything else) 
        // from accessing tiles in this layer ( reading or writing ). The layer should be locked 
        // into it's currentstate until it's shown again.
        /// <summary>
        /// Hide a certain layer of tiles.
        /// </summary>
        /// <param name="layer"></param>
        public void HideLayer( SortingLayer layer )
        {
            isLayerVisible_.Set(layer, false );
            mesh_.HideLayer(layer);
            //ChunkMesh.HideLayer(layer);
        }

        public void ShowLayer( SortingLayer layer )
        {
            isLayerVisible_.Set(layer, true);
            mesh_.ShowLayer(layer);
        }

       // public void SetTileLocal(  )

        public void Print()
        {
            foreach( var p in this )
            {
                Debug.LogFormat("{0}: {1}", p.Key, p.Value.name);
            }
        }


        public Dictionary<TileIndex,Tile>.Enumerator GetEnumerator()
        {
            return indexDict_.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TileIndex, Tile>> IEnumerable<KeyValuePair<TileIndex, Tile>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [System.Serializable]
        class TileList : List<Tile> { }

        [System.Serializable]
        class TilesToIntVector2Dict : SerializableDictionary<IntVector2, TileList> { }

        [System.Serializable]
        class TileToIndexDict : SerializableDictionary<TileIndex, Tile> { }

        [System.Serializable]
        class TileToIntDict : SerializableDictionary<int, Tile> { }

        
    }

}