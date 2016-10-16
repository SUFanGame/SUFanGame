using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class World : MonoBehaviour
{
    static World world_ = null;
    //public static World Instance
    //{
    //    get
    //    {
    //        world_ = GameObject.FindObjectOfType<World>();
    //        if( world_ == null )
    //        {
    //            world_ = new GameObject("TileMap").AddComponent<World>();
    //        }
    //    }
    //    private set
    //    {
    //    }
    //}

    void OnEnable()
    {
        //Instance = this;
    }

    [SerializeField]
    //[HideInInspector]
    TileMap tileMap_ = new TileMap();
    
    public TileMap TileMap { get { return tileMap_; } }

}
