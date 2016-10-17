using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Entities.EntityDrivers;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Overworld
{
    public class ChunkRenderer : MonoBehaviour
    {
        public static ChunkRenderer AddChunkRenderer(GameObject target, bool chunkPositionIsAbsolute, Chunk source)
        {
            //Debug.Log(source.AppDataPath);
            ChunkRenderer addedChunkRenderer = target.AddComponent<ChunkRenderer>();
            addedChunkRenderer.ChunkPositionIsAbsolute = chunkPositionIsAbsolute;
            addedChunkRenderer.SourceChunk = source;
            return addedChunkRenderer;
        }

        private bool chunkPositionIsAbsolute = false;
        private Chunk sourceChunk;
        private Chunk displayChunk;
        private EntityDriver entityDriver;

        private Vector3 pivotPoint;
        private bool spriteRenderersSet = true;

        public bool ChunkPositionIsAbsolute
        {
            get { return chunkPositionIsAbsolute; }
            set { chunkPositionIsAbsolute = value; }
        }

        public Chunk SourceChunk
        {
            get { return sourceChunk; }
            //TODO should this be publicly settable?
            set
            {
                sourceChunk = value;
                DisplayChunk = SourceChunk;
            }
        }

        public Chunk DisplayChunk
        {
            get
            {
                //HACK
                if (displayChunk == null)
                {
                    displayChunk = new Chunk(new TileInstance[] {}, new GroupInstance[] {});
                }

                return displayChunk;
            }
            set
            {
                displayChunk = value;
                RenderDisplayChunk();
            }
        }

        private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            //GameController.Instance.RegisterActiveChunkRenderer(this);
            entityDriver = GetComponent<EntityDriver>();
        }

        protected virtual void Update()
        {
            if (!spriteRenderersSet)
            {
                SetSpriteRenderers();
            }
            StartUpdateSpriteRenderers();
        }

        protected virtual void FixedUpdate()
        {
        }

        protected virtual void OnDestroy()
        {
            //GameController.Instance.DeregisterActiveChunkRenderer(this);
        }

        public void UpdatePivotPoint()
        {
            //If the Chunk's Position is an absolute value, move to it and set the pivot to is
            if (ChunkPositionIsAbsolute)
            {
                transform.position = DisplayChunk.Position;
                pivotPoint = DisplayChunk.Position;
            }
            //Otherwise, set the pivot to zero and keep the current position
            else
            {
                pivotPoint = Vector3.zero;
            }
        }

        public void RenderDisplayChunk()
        {
            UpdatePivotPoint();

            if (!DisplayChunk.Rendered)
            {
                DisplayChunk.RenderChunkLayers();
                spriteRenderersSet = false;
            }
            else
            {
                spriteRenderersSet = false;
            }

            //TODO I moved this from the top of this method to the bottom. Are there any side effects?
            //Reset any Asynchronous Chunk Layer animations
            DisplayChunk.ResetAsynchronousChunkLayers();
        }

        private void SetSpriteRenderers()
        {
            spriteRenderersSet = true;

            //Cache a reference to the CurrentChunk's ChunkLayers
            Chunk.ChunkLayer[] chunkLayers = DisplayChunk.ChunkLayers;

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sprite = null;
            }

            //Make sure there are enough SpriteRenderers for all the ChunkLayers
            if (spriteRenderers.Count < chunkLayers.Length)
            {
                int spriteRenderersNeeded = chunkLayers.Length - spriteRenderers.Count;
                for (int i = 0; i < spriteRenderersNeeded; i++)
                {
                    //Create a GameObject for the ChunkLayer
                    GameObject child = new GameObject("Chunk Layer");
                    child.transform.position = gameObject.transform.position;
                    child.transform.SetParent(gameObject.transform);

                    //Add a SpriteRenderer to the GameObject with the ChunkLayer's Sprite
                    SpriteRenderer sr = child.AddComponent<SpriteRenderer>();
                    sr.sortingLayerName = "Overworld";

                    //Store the SpriteRenderer in a List of all the SpriteRenderers
                    spriteRenderers.Add(sr);
                }
            }

            //Set each SpriteRenderer up to match a ChunkLayer
            for (int i = 0; i < chunkLayers.Length; i++)
            {
                Chunk.ChunkLayer chunkLayer = DisplayChunk.ChunkLayers[i];

                //TODO why does this 'if' block fix the disappearing chunks problem?
                if (chunkLayer.SpriteRenderer == null)
                {
                    chunkLayer.SpriteRenderer = spriteRenderers[i];
                    ChunkLayerMonitor clm = spriteRenderers[i].gameObject.AddComponent<ChunkLayerMonitor>();
                    clm.chunkLayer = chunkLayer;
                    clm.spriteRenderer = spriteRenderers[i];
                }
            }
        }

        private void StartUpdateSpriteRenderers()
        {
            StartCoroutine(UpdateSpriteRenderers());
        }

        private IEnumerator UpdateSpriteRenderers()
        {
            if (DisplayChunk != null)
            {
                foreach (Chunk.ChunkLayer chunkLayer in DisplayChunk.ChunkLayers)
                {
                    chunkLayer.UpdateSpriteRenderer();
                    //Allow for a pause if there is no entity controller because it's not critical
                    if (entityDriver == null)
                    {
                        yield return null;
                    }
                }

                UpdateSortingOrder();
            }
        }

        private void UpdateSortingOrder()
        {
            if (DisplayChunk == null)
            {
                return;
            }
            //Set each SpriteRenderer up to match a ChunkLayer
            for (int i = 0; i < DisplayChunk.ChunkLayers.Length; i++)
            {
                Chunk.ChunkLayer chunkLayer = DisplayChunk.ChunkLayers[i];
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                spriteRenderer.sortingOrder = ((SourceChunk.CurrentElevation + chunkLayer.Signature.Elevation)*100) +
                                              chunkLayer.Signature.SortingOrder;
            }
        }

        public TileInstance[] GetTileInstancesAtPosition(int x, int y)
        {
            //Caclulate the true X&Y values by subtracting the transform's distance from the pivot from the X&Y values
            int trueX = x - Mathf.RoundToInt(transform.position.x - pivotPoint.x);
            int trueY = y - Mathf.RoundToInt(transform.position.y - pivotPoint.y);

            return DisplayChunk.AllInstancesFlattenedCoordinated.Get(trueX, trueY);
        }

        public void SetVisibility(bool visible)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.gameObject.SetActive(visible);
            }
        }
    }
}