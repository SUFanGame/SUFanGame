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
    EditorInstanceMap tileMap_ = new EditorInstanceMap();
    
    public EditorInstanceMap TileMap { get { return tileMap_; } }

}
