using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.World;

public class GameStarter : MonoBehaviour 
{
    void Update()
    {

        var grid = FindObjectOfType<Grid>();
        var map = FindObjectOfType<Map>();
        grid.BuildFromMap(map);

        enabled = false;
    }
}
