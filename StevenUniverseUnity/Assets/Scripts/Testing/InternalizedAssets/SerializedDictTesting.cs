using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;

[ExecuteInEditMode]
public class SerializedDictTesting : MonoBehaviour 
{

    [SerializeField]
    ChunkStackToPos dict_ = new ChunkStackToPos();

    public Tile tile_;
    public IntVector2 position_;

    [ContextMenu("Insert")]
    void Insert()
    {
        TileList list;
        if (!dict_.TryGetValue(position_, out list))
        {
            dict_[position_] = list = new TileList();
        }
        list.Add(tile_);
    }

    [ContextMenu("Print")]
    void Print()
    {
        foreach (var pair in dict_)
        {
            Debug.LogFormat("{0} : IsNull : {1}", pair.Key, pair.Value == null);
        }
    }


    [System.Serializable]
    class TileList : List<Tile> { }

    [System.Serializable]
    class ChunkStackToPos : SerializableDictionary<IntVector2, TileList> { }
}
