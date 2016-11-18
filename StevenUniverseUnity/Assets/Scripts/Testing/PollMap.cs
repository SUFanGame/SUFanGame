using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Util;

public class PollMap : MonoBehaviour 
{
    LoadSceneChunks loader;
    
    void Awake()
    {
        loader = GameObject.FindObjectOfType<LoadSceneChunks>();
    }

    void Update()
    {
        if( Input.GetMouseButtonDown(0 ) )
        {
            IntVector2 pos = (IntVector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var tiles = loader.tileMap_.GetTileStack(pos.x, pos.y);

            if( tiles != null )
            {
                foreach( var tile in tiles )
                {
                    
                    Debug.LogFormat((tile as TileInstance).TileTemplate.Name);
                }
            }
        }
    }
}
