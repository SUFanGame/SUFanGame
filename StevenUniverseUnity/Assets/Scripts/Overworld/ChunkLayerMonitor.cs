using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    public class ChunkLayerMonitor : MonoBehaviour
    {
        public Chunk.ChunkLayer chunkLayer;
        public SpriteRenderer spriteRenderer;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (chunkLayer.SpriteRenderer != spriteRenderer)
            {
                //Debug.Log("weiugbswieubgswerh");
            }
        }
    }
}