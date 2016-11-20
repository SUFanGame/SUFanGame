using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.MapEditing;

namespace StevenUniverse.FanGame.World
{
    // TODO : Chunks should maintain a list of Dynamic Meshes, one for each Sorting Layer.
    // This may be better off in a separate class ( ChunkMesh ) to avoid complicating this one
    // any more than it already is.

    // TODO : Layers can be hidden or shown via the map editor. The chunk should ignore ANY operations attempted
    // on a "hidden" layer. The Map class will follow these rules as well. It may be best to have the layer states set up in
    // SortingLayerUtil, so the states can be access globally.
    // Layers will be hidden visually as well. There's a few options - since Meshes are already divided by sorting layers it's probably
    // easiest to just iterate through all chunks and hide/show the matching layer for each chunk.
    // Could also use Unity's camera culling: http://answers.unity3d.com/questions/561274/using-layers-to-showhide-different-players.html
    // Note in both the latter examples this would require setting up Layers that match the SortingLayers and having the meshes be set to the
    // appropriate Layer.


    /// <summary>
    /// A chunk of tiles, basically representing a small portion of one cross-section of the map ( where the cross sections are divided by height ).
    /// The map maintains a structure of chunks and creates them as they are need, as tiles are added to the map.
    /// For any 2D position of the chunk there can exist a stack of tiles: one tile for each sorting layer set up in the project.
    /// </summary>
    [RequireComponent(typeof(ChunkMesh)), ExecuteInEditMode]
    public class Chunk : MonoBehaviour, IEnumerable<KeyValuePair<TileIndex,Tile>>
    {
        /// <summary>
        /// Dictionary mapping stacks of tiles (where each index of the stack represents a sorting layer ) to their 3D position in local space.
        /// </summary>
        [SerializeField]
        TilesToIntVector2Dict tileStackDict_ = new TilesToIntVector2Dict();

        /// <summary>
        /// Dictionary mapping tiles directly to their tile index ( Position & SortingLayer )
        /// </summary>
        [SerializeField]
        TileToIndexDict indexDict_ = new TileToIndexDict();

        [SerializeField]
        SortingLayerVisibility isLayerVisible_ = new SortingLayerVisibility();

        ChunkMesh mesh_ = null;
        public ChunkMesh Mesh { get { return mesh_; } }

        void Awake()
        {
            isLayerVisible_.Awake();
            mesh_ = GetComponent<ChunkMesh>();
        }

        /// <summary>
        /// Retrieve a single tile from the local position and sorting layer.
        /// </summary>
        public Tile GetTileLocal( IntVector3 localPosition, SortingLayer layer )
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
        public Tile GetTileWorld(IntVector3 worldPos, SortingLayer layer)
        {
            if (!isLayerVisible_.Get(layer))
            {
                return null;
            }

            return GetTileLocal((IntVector3)transform.position - worldPos, layer);
        }

        /// <summary>
        /// Retrieve a list of tiles where each index of the list represents a SortingLayer.
        /// The given position should be LOCAL to the chunk. Note that raw SortingLayer.Value can start
        /// below zero, see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        public List<Tile> GetTilesLocal( IntVector2 localPos )
        {
            // TODO: How to account for hidden layers here? Maybe we should pass in a buffer that gets populated,
            // so we can selectively ignore tiles
            List<Tile> tiles;
            if (!tileStackDict_.TryGetValue(localPos, out tiles))
            {
                // Ensure retrieved tile stacks are always populated with null refs
                tileStackDict_[localPos] = tiles = new List<Tile>();
                for (int i = 0; i < SortingLayerUtil.LayerCount; ++i)
                {
                    tiles.Add(null);
                }
            }
            return tiles;
        }

        /// <summary>
        /// Retrieve a list of tiles where each index of the list represents a SortingLayer
        /// Note that raw SortingLayer.Value can start below zero, 
        /// see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        public List<Tile> GetTilesWorld( IntVector2 worldPos )
        {
            return GetTilesLocal((IntVector2)transform.position - worldPos);
        }

        public void SetTileWorld( TileIndex index, Tile t )
        {
            if (!isLayerVisible_.Get(t.DefaultSortingLayer_))
            {
                return;
            }

            //Debug.LogFormat("Setting tile at {0}", index);
            var oldPos = index.Position_;
            index.Position_ = index.Position_ - (IntVector3)transform.localPosition;
            //Debug.LogFormat("After conversion ({0} - {1}): {2}", oldPos, index.Position_, (IntVector3)transform.localPosition);
            SetTileLocal(index, t);
        }

        public void SetTileLocal( TileIndex index, Tile t )
        {
            if (!isLayerVisible_.Get(t.DefaultSortingLayer_))
            {
                return;
            }

            if ( t == null )
            {
                // Remove the tile from the mesh
            }
            // Set the tile in the index dict
            indexDict_[index] = t;

            // Set the tile in the stack dict
            var tiles = GetTilesLocal((IntVector2)index.Position_);
            tiles[SortingLayerUtil.GetLayerIndex(index.Layer_)] = t;
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
            isLayerVisible_.Set(layer, true);
            mesh_.HideLayer(layer);
            //ChunkMesh.HideLayer(layer);
        }

        public void ShowLayer( SortingLayer layer )
        {
            isLayerVisible_.Set(layer, false);
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
        class TilesToIntVector2Dict : SerializableDictionary<IntVector2, List<Tile>> { }

        [System.Serializable]
        class TileToIndexDict : SerializableDictionary<TileIndex, Tile> { }

        [System.Serializable]
        class TileToIntDict : SerializableDictionary<int, Tile> { }

        
    }

}