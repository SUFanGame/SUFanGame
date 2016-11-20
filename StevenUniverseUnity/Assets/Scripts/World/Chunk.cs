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
    /// </summary>
    [RequireComponent(typeof(ChunkMesh))]
    public class Chunk : MonoBehaviour, IEnumerable<KeyValuePair<TileIndex,Tile>>
    {
        /// <summary>
        /// Dictionary mapping stacks of tiles (where each index of the stack == Tile.SortingLayer.Value ) to their 3D position in local space.
        /// </summary>
        [SerializeField]
        TilesToIntVector2Dict tileStackDict_ = new TilesToIntVector2Dict();

        /// <summary>
        /// Dictionary mapping tiles directly to their tile index ( Position & SortingLayer )
        /// </summary>
        [SerializeField]
        TileToIndexDict indexDict_ = new TileToIndexDict();

        /// <summary>
        /// Retrieve a single tile from the local position and sorting layer.
        /// </summary>
        public Tile GetTileLocal( IntVector3 localPosition, SortingLayer layer )
        {
            Tile t;
            indexDict_.TryGetValue(new TileIndex(localPosition, layer), out t);
            return t;
        }

        /// <summary>
        /// Retrieve a single tile from the chunk via the given world position and sorting layer.
        /// </summary>
        public Tile GetTileWorld(IntVector3 worldPos, SortingLayer layer)
        {
            return GetTileLocal((IntVector3)transform.position - worldPos, layer);
        }

        /// <summary>
        /// Retrieve a list of tiles where each index of the list represents a SortingLayer (SortingLayer.Value).
        /// The given position should be LOCAL to the chunk. Note that raw SortingLayer.Value can start
        /// below zero, see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        public List<Tile> GetTilesLocal( IntVector2 localPos )
        {
            List<Tile> tiles;
            if (!tileStackDict_.TryGetValue(localPos, out tiles))
            {
                tileStackDict_[localPos] = tiles = new List<Tile>();
                // Ensure tile stacks are always populated with null refs
                for (int i = 0; i < SortingLayerUtil.LayerCount; ++i)
                {
                    tiles.Add(null);
                }
            }
            return tiles;
        }

        /// <summary>
        /// Retrieve a list of tiles where each index of the list represents a SortingLayer (SortingLayer.Value).
        /// Note that raw SortingLayer.Value can start below zero, 
        /// see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        public List<Tile> GetTilesWorld( IntVector2 worldPos )
        {
            return GetTilesLocal((IntVector2)transform.position - worldPos);
        }

        public void SetTileWorld( TileIndex index, Tile t )
        {
            //Debug.LogFormat("Setting tile at {0}", index);
            var oldPos = index.Position_;
            index.Position_ = index.Position_ - (IntVector3)transform.localPosition;
            //Debug.LogFormat("After conversion ({0} - {1}): {2}", oldPos, index.Position_, (IntVector3)transform.localPosition);
            SetTileLocal(index, t);
        }

        public void SetTileLocal( TileIndex index, Tile t )
        {
            if( t == null )
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
            // ChunkMesh.HideLayer(layer);
        }

        public void ShowLayer( SortingLayer layer )
        {
            //ChunkMesh.ShowLayer(layer)
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