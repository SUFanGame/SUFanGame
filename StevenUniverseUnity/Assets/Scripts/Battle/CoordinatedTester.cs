using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;

public class CoordinatedTester : MonoBehaviour 
{
    CoordinatedList<TileInstance> list = new CoordinatedList<TileInstance>();

    void Awake()
    {
        list.Add(new TileInstance("", 0, 5, 15));
        list.Add(new TileInstance("", 0, 5, 11));
        list.Add(new TileInstance("", 0, 5, 10));
        list.Add(new TileInstance("", 0, 5, 9));

        Debug.LogFormat(list.Get(0, 5).Count.ToString() );
    }
}
