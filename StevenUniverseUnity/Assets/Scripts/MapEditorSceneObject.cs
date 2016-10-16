using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Wrapper for a tile map so it can be used within a scene from the map editor.
/// Unity editors cannot properly maintain references to monobehaviours within a scene 
/// through serialization, so we need to have a scene object to hold those references. 
/// The map editor can safely maintain a reference to this gameobject.
/// </summary>
public class MapEditorSceneObject : MonoBehaviour 
{
    public TileMap map_ = new TileMap();
}
