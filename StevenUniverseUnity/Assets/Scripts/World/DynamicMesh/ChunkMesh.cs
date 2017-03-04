using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SUGame.Util.MapEditing;
using System.Linq;
using SUGame.Util;
using SUGame.Util.Common;

namespace SUGame.World.DynamicMesh
{
    /// <summary>
    /// A Chunk mesh that maintains a stack of tiled meshes, where each layer of the stack represents the
    /// sorting layers of the project.
    /// </summary>
    [ExecuteInEditMode]
    public class ChunkMesh : MonoBehaviour
    {
        [SerializeField]
        TiledMesh[] meshes_ = null;

        [SerializeField, HideInInspector]
        IntVector2 size_;
        
        /// <summary>
        /// The size of each of the tiled meshes.
        /// </summary>
        public IntVector2 Size_
        {
            get
            {
                return size_;
            }
            set
            {
                size_ = value;
            }
        }

        void Awake()
        {
            if( meshes_ == null )
                meshes_ = new TiledMesh[EnumUtil.GetEnumValues<TileLayer>().Count];
        }
        
        /// <summary>
        /// Create a TiledMesh with the given Sorting Layer, parent, and size.
        /// </summary>
        public TiledMesh CreateLayerMesh( TileLayer tileLayer, IntVector2 size, Material material )
        {
            int overworldLayerValue = SortingLayer.GetLayerValueFromName("Overworld");
            var go = new GameObject(tileLayer.ToString() + " Mesh");
            var mesh = go.AddComponent<TiledMesh>();
            mesh.TileLayer_ = tileLayer;
            go.transform.SetParent(transform, false);
            go.transform.SetSiblingIndex(overworldLayerValue);
            mesh.Size_ = size;
            mesh.renderer_.sharedMaterial = material;

            int elevation = (int)transform.position.z;
            mesh.renderer_.sortingLayerName = "Overworld";
            mesh.renderer_.sortingOrder = tileLayer.GetSortingOrder(elevation);

            meshes_[(int)tileLayer] = mesh;
            mesh.ImmediateUpdate();
            //Debug.LogFormat("Creating mesh of size {0}", size);

            return mesh;
        }


        /// <summary>
        /// Retrieve the tiled mesh matching the given sorting layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public TiledMesh GetMesh( TileLayer layer )
        {
            int index = (int)layer;

            return GetLayerMesh(index);
        }

        /// <summary>
        /// Retrieve the Tiled Mesh matching the given zero based sorting layer index.
        /// </summary>
        public TiledMesh GetLayerMesh( int index )
        {
            return meshes_[index];
        }

        public void HideLayer( TileLayer layer )
        {
            int layerIndex = (int)layer;

            var mesh = meshes_[layerIndex];
            if (mesh != null)
                mesh.renderer_.enabled = false;
        }

        public void ShowLayer( TileLayer layer )
        {
            int layerIndex = (int)layer;

            var mesh = meshes_[layerIndex];
            if (mesh != null)
                mesh.renderer_.enabled = true;
        }

        /// <summary>
        /// Refresh uv and color data for all layers in this chunk.
        /// </summary>
        public void RefreshLayers()
        {
            for( int i = 0; i < meshes_.Length; ++i )
            {
                if (meshes_[i] != null)
                {
                    meshes_[i].RefreshUVs();
                    meshes_[i].RefreshColors();
                    //meshes_[i].ImmediateUpdate();
                }
            }
        }
    }
}
