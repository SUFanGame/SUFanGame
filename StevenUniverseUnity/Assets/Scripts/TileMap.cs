using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using StevenUniverse.FanGame.Battle;
using StevenUniverse.FanGame.Overworld;

///// <summary>
///// A map of all the tiles. Tiles can be accessed individually by tile index or by 2D position which
///// will return all the tiles at that position.
///// </summary>
//[System.Serializable]
//public class TileMap<T> : ISerializationCallbackReceiver, IEnumerable<T> where T : ICoordinated
//{
//    // Map stacks of tiles to their 2D position.
//    Dictionary<IntVector2, List<T>> tileStackDict_ = new Dictionary<IntVector2, List<T>>();
//    // Map tiles directly to their tile index (Position, Elevation, Layer)
//    Dictionary<TileIndex, T> tileIndexDict_ = new Dictionary<TileIndex, T>();

//    /// <summary>
//    /// Build a tile map from a list of all the map's tiles.
//    /// </summary>
//    public TileMap( T[] tileArray )
//    {

//    }

//    void Add( T t )
//    {
//        //IntVector2 pos = (IntVector2)t.Position;
//        //int elevation = 
//    }

//    public IEnumerator<T> GetEnumerator()
//    {
//        throw new NotImplementedException();
//    }

//    public void OnAfterDeserialize()
//    {
//        throw new NotImplementedException();
//    }

//    public void OnBeforeSerialize()
//    {
//        throw new NotImplementedException();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        throw new NotImplementedException();
//    }
//}
