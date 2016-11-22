using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.MapEditing;
using System.Linq;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.World
{
    /// <summary>
    /// A Chunk mesh that maintains a stack of tiled meshes, where each layer of the stack represents the
    /// sorting layers of the project.
    /// </summary>
    [ExecuteInEditMode]
    public class ChunkMesh : MonoBehaviour
    {
        [SerializeField,HideInInspector]
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
                VerifyMeshValues();
            }
        }

        void Awake()
        {
            meshes_ = new TiledMesh[SortingLayer.layers.Length];
        }
        
        TiledMesh CreateMesh( int layerIndex )
        {
            var layer = SortingLayerUtil.GetLayerFromIndex(layerIndex);
            var go = new GameObject(layer.name + " Mesh");
            var mesh = go.AddComponent<TiledMesh>();
            mesh.SortingLayer_ = layer;
            go.transform.SetParent(transform, false);
            go.transform.SetSiblingIndex(layerIndex);

            return mesh;
        }

        /// <summary>
        /// Iterate through the meshes and ensure their size and layer values are correct for this chunk mesh.
        /// </summary>
        void VerifyMeshValues()
        {
            for( int i = 0; i < meshes_.Length; ++i )
            {
                var mesh = meshes_[i];
                
                if( mesh.Size_ != size_ )
                {
                    mesh.Size_ = size_;
                }
                
                mesh.SortingLayer_ = SortingLayerUtil.GetLayerFromIndex(i);
                mesh.transform.SetSiblingIndex(i);
            }
        }

        /// <summary>
        /// Retrieve the tiled mesh matching the given sorting layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public TiledMesh GetMesh( SortingLayer layer )
        {
            int index = SortingLayerUtil.GetLayerIndex(layer);

            // Create the mesh for this layer if it doesn't already exist.
            if (meshes_[index] == null)
                meshes_[index] = CreateMesh(index);
            return meshes_[index];
        }

        /// <summary>
        /// Retrieve the Tiled Mesh matching the given zero based sorting layer index.
        /// </summary>
        public TiledMesh GetMesh( int layerIndex )
        {
            return GetMesh(SortingLayerUtil.GetLayerFromIndex(layerIndex));
        }

        public void HideLayer( SortingLayer layer )
        {
            int layerIndex = SortingLayerUtil.GetLayerIndex(layer);

            var mesh = meshes_[layerIndex];
            if (mesh != null)
                mesh.renderer_.enabled = false;
        }

        public void ShowLayer( SortingLayer layer )
        {
            int layerIndex = SortingLayerUtil.GetLayerIndex(layer);

            var mesh = meshes_[layerIndex];
            if (mesh != null)
                mesh.renderer_.enabled = false;
        }
    }
}
