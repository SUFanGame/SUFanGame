using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using System.Linq;

// TODO : So. Lists of objects as TVALUE simply will not serialize. What can we do? 

[ExecuteInEditMode]
public class SerializedDictTesting : MonoBehaviour 
{

    [SerializeField]
    //ChunkStackToPos dict_ = new ChunkStackToPos();
    IntDict dict_ = new IntDict();

    [SerializeField]
    List<Tile> tiles_ = new List<Tile>();

    // public Tile tile_;
    public int value_;
    public IntVector2 position_;

    [ContextMenu("Insert")]
    void Insert()
    {
        //TileList list;
        IntList list;
        if (!dict_.TryGetValue(position_, out list))
        {
            //dict_[position_] = list = new TileList();
            dict_[position_] = list = new IntList();
        }
        //list.Add(tile_);
        list.Add(value_);
    }

    [ContextMenu("Print")]
    void Print()
    {
    }

    void OnGUI()
    {
        foreach( var pair in dict_ )
        {
            string listString = pair.Value == null ? "Null" : string.Join(",", pair.Value.Select(l => l.ToString()).ToArray());

            var str = string.Format("{0} :  {1}", pair.Key, listString);
            GUILayout.Label(str);
        }
    }


    [System.Serializable]
    class TileList : List<Tile> { }

    [System.Serializable]
    class ChunkStackToPos : SerializableDictionary<IntVector2, TileList> { }

    [System.Serializable]
    class IntList : List<int> { }

    [System.Serializable]
    class IntDict : SerializableDictionary<IntVector2, IntList> { }


}
