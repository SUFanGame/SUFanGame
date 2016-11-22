using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;
using System.Linq;

// TODO : So. Lists of objects as TVALUE simply will not serialize. What can we do? 

[ExecuteInEditMode]
public class SerializedDictListsTesting : MonoBehaviour 
{

    [SerializeField]
    //ChunkStackToPos dict_ = new ChunkStackToPos();
    IntsDict dict_ = new IntsDict();

    [SerializeField]
    List<Tile> tiles_ = new List<Tile>();

    // public Tile tile_;
    public int key_;
    public int value_;

    [ContextMenu("Insert")]
    void Insert()
    {
        List<int> list;
        if( !dict_.TryGetValue(key_, out list) )
        {
            dict_[key_] = list = new List<int>();
        }
        list.Add(value_);
    }

    [ContextMenu("Print")]
    void Print()
    {
        var enumerator = dict_.ListEnumerator();
        while (enumerator.MoveNext())
        {
            var pair = enumerator.Current;
            var list = pair.Value;
            string listString = string.Join(",", list.Select(v => v.ToString()).ToArray());
            Debug.LogFormat("{0} : {1}", pair.Key, listString);
        }
    }

    void OnGUI()
    {
        var enumerator = dict_.ListEnumerator();
        while( enumerator.MoveNext() )
        {
            var pair = enumerator.Current;
            var list = pair.Value;
            string listString = string.Join(",", list.Select(v => v.ToString()).ToArray());
            GUILayout.Label(string.Format("{0} : {1}", pair.Key, listString));
        }
    }


    [System.Serializable]
    class IntsDict : SerializableDictionaryOfLists<int, int> { }


}
