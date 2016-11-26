using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.MapEditing;
using System.Linq;

namespace StevenUniverse.FanGame.World
{
    
    
    /// <summary>
    /// A chunk of tiles, basically representing a small portion of one cross-section of the map ( where the cross sections are divided by height ).
    /// The map maintains a structure of chunks and creates them as they are need, as tiles are added to the map.
    /// For any 2D position of the chunk there can exist a stack of tiles: one tile for each sorting layer set up in the project.
    /// </summary>
    [RequireComponent(typeof(ChunkMesh)), ExecuteInEditMode]
    public class Chunk : MonoBehaviour, IEnumerable<KeyValuePair<TileIndex,Tile>>
    {
        public const int MAXSIZE = 50;

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

        


        /// <summary>
        /// The height (z-position) of the chunk in world space.
        /// </summary>
        public int Height_ { get { return (int)transform.position.z; } }

        public Material terrainMaterial_ = null;

        public bool IsEmpty
        {
            get { return indexDict_.Count == 0; }
        }

        [SerializeField,HideInInspector]
        IntVector2 size_;
        public IntVector2 Size_
        {
            get
            {
                return size_;
            }
            set
            {
                value = IntVector2.Clamp(value, 1, int.MaxValue);
                size_ = value;
                var pos = transform.localPosition;
                int x = Mathf.FloorToInt(pos.x / (float)size_.x);
                int y = Mathf.FloorToInt(pos.y / (float)size_.y);
                int z = (int)pos.z;
                chunkIndex_ = new IntVector3(x, y, z);
            }
        }

        [SerializeField]
        IntVector3 chunkIndex_;
        public IntVector3 Chunkindex_
        {
            get { return chunkIndex_; }
        }

        void Awake()
        {
            //Debug.LogFormat("Setting awake on LayerVisbility in {0}", name);
            isLayerVisible_.Awake();
            
            mesh_ = GetComponent<ChunkMesh>();
        }

        void OnEnable()
        {

        }

        /// <summary>
        /// Returns the topmost tile (in terms of sorting layers) at the given position.
        /// Returns null if no visible tiles exist at the given location.
        /// </summary>
        /// <param name="localPos">The 2D position to search, local to this chunk.</param>
        /// <param name="layer">Which layer a tile was found at, if any.</param>
        public Tile GetTopTileLocal( IntVector2 localPos, out SortingLayer layer )
        {
            layer = default(SortingLayer);

            if (!sortingLayerDict_.ContainsKey(localPos))
            {
                //Debug.LogFormat("Local Pos {0} doesn't contain a tilestack?", localPos);
                return null;
            }

            var stack = GetTileStack(localPos);

            for( int i = stack.Count - 1; i >= 0; --i )
            {
                layer = SortingLayerUtil.GetLayerFromIndex(i);
                // Check if this layer is enabled 
                if ( isLayerVisible_.Get(layer) && stack[i] != null )
                {
                    //Debug.LogFormat("sETTING LAYER TO {0}", layer.name);
                    return stack[i];
                }
            }

            return null;
        }


        public Tile GetTopTileWorld( IntVector2 worldPos, out SortingLayer layer )
        {
            layer = default(SortingLayer);
            return GetTopTileLocal( worldPos - (IntVector2)transform.position, out layer );
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
            //Debug.LogFormat("Getting tile from chunk {0} at {1} {2}", name, localPosition, layer.name );
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

            return GetTileLocal(worldPos - (IntVector2)transform.position, layer);
        }

        /// <summary>
        /// Retrieve a list of tiles where each index of the list represents a SortingLayer.
        /// The given position should be LOCAL to the chunk. Note that raw SortingLayer.Value can start
        /// below zero, see <seealso cref="SortingLayerUtil.GetLayerIndex(SortingLayer)"/>
        /// </summary>
        private List<Tile> GetTileStack( IntVector2 localPos )
        {
            TileListWrapper wrapper;
            if (!sortingLayerDict_.TryGetValue(localPos, out wrapper))
            {
                // Ensure retrieved tile stacks are always populated with null refs
                sortingLayerDict_[localPos] = wrapper = new TileListWrapper();
                for (int i = 0; i < SortingLayerUtil.LayerCount; ++i)
                {
                    wrapper.list_.Add(null);
                }
            }
            return wrapper.list_;
        }


        public void SetTileWorld( TileIndex index, Tile t )
        {
            //Debug.LogFormat("SetTileWorld index: {0}", index);
            if ( !isLayerVisible_.Get(index.Layer_) )
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

            //Debug.LogFormat("Index: {0}, stackSize: {1}", index, stack.Count);

            var layerIndex = index.SortingLayerIndex_;
            stack[layerIndex] = t;
            // TODO : Write to mesh
            var layerMesh = GetLayerMesh(index.Layer_);

            layerMesh.SetUVs(index.position_, t.Sprite_.uv);
            //layerMesh.SetColors(index.position_, Color.white);
            layerMesh.SetVisible(index.position_);

            layerMesh.ImmediateUpdate();
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
            //Debug.LogFormat("Erasing tile at {0}", pos);
            // First check if there are any tiles here
            TileListWrapper wrapper;
            if( !sortingLayerDict_.TryGetValue(pos, out wrapper) )
            {
                return;
            }
            var stack = wrapper.list_;

            for( int i = stack.Count - 1; i >= 0; --i )
            {
                var t = stack[i];
                var layer = SortingLayerUtil.GetLayerFromIndex(i);
                // Ignore layers that aren't visible
                if (t == null || !isLayerVisible_.Get(layer))
                    continue;

                EraseTile(new TileIndex(pos, layer));
                return;
            }
        }

        public void EraseTileWorld( IntVector2 pos, SortingLayer layer )
        {
            pos -= (IntVector2)transform.position;
            EraseTile(new TileIndex(pos, layer));
        }
        

        void EraseTile( TileIndex index )
        {
            if (!indexDict_.ContainsKey(index))
            {
                //Debug.LogFormat("Index {0} not present in {1}", index, name);
                return;
            }
            //Debug.LogFormat("Erasing tile at {0}", index);
            indexDict_.Remove(index);
            sortingLayerDict_[index.position_].list_[index.SortingLayerIndex_] = null;

            var layerMesh = GetLayerMesh(index.Layer_);

            //layerMesh.SetColors(index.position_, default(Color32));
            layerMesh.SetHidden(index.position_);

            layerMesh.ImmediateUpdate();
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

        /// <summary>
        /// Sets the visible alpha value for all chunks in this mesh.
        /// </summary>
        /// <param name="val"></param>
        public void SetVisibleAlpha( byte cutoffAlpha )
        {
            //Debug.Log("Setting visible alpha for chunk " + name);
            var layers = SortingLayer.layers;
            for (int i = 0; i < layers.Length; ++i)
            {
                var mesh = mesh_.GetLayerMesh(i);
                if (mesh == null)
                    continue;

                mesh.SetVisibleAlpha(cutoffAlpha);
            }
        }

        /// <summary>
        /// Retrieve the TiledMesh for the given sorting layer.
        /// The mesh will be created if none exists.
        /// </summary>
        public TiledMesh GetLayerMesh( SortingLayer layer )
        {
            int layerIndex = SortingLayerUtil.GetLayerIndex(layer);

            var mesh = mesh_.GetLayerMesh(layerIndex);

            if (mesh == null)
                mesh = mesh_.CreateLayerMesh(layerIndex, size_, terrainMaterial_);

            return mesh;
        }

        public void RefreshMesh()
        {
            mesh_.RefreshLayers();
        }

       // public void SetTileLocal(  )

        public void Print()
        {
            foreach( var p in this )
            {
                if( p.Value == null )
                {
                    Debug.LogFormat("Print is trying to read value at {0}, but it's null", p.Key);
                }
                Debug.LogFormat("Chunk: {0}: Index: {1} Tile : {2}", name, p.Key, p.Value.name);
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

        //[System.Serializable]
        //class TilesToIntVector2Dict : SerializableDictionaryOfLists<IntVector2, Tile> { }

        [System.Serializable]
        class TileListWrapper { public List<Tile> list_ = new List<Tile>(); }

        [System.Serializable]
        class TilesToIntVector2Dict : SerializableDictionary<IntVector2, TileListWrapper> { }

        [System.Serializable]
        class TileToIndexDict : SerializableDictionary<TileIndex, Tile> { }
        
    }

}