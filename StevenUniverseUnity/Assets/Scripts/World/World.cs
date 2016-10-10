using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class World : MonoBehaviour
{
    public static World Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;
    }

    [SerializeField]
    //[HideInInspector]
    TileMap tileMap_ = new TileMap();
    
    public TileMap TileMap { get { return tileMap_; } }

}
