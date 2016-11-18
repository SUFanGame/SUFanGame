using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;

namespace StevenUniverse.FanGame.World
{

    /// <summary>
    /// A stack of chunks where each layer of the stack represents a SortingLayer (SortingLayer.ID)
    /// </summary>
    [System.Serializable]
    public class ChunkStack : SerializableDictionary<int, Chunk>
    {
        
    }

}