using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.MapSkirmish;
using System;

public class CoordinatedTester : MonoBehaviour 
{
    class Tile : ITile
    {
        public Tile ( string name, int x, int y, int elevation, int sortingOrder )
        {
            Position = new IntVector2(x,y);
            Elevation = elevation;
            SortingOrder = sortingOrder;
            name_ = name;
        }

        public string name_;

        public int Elevation
        {
            get; set;
        }

        public IntVector2 Position
        {
            get; set;
        }

        public int SortingOrder
        {
            get; set;
        }

        public string TileModeName
        {
            get; set;
        }

        public bool IsGrounded
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} - {2}", name_, Elevation, SortingOrder);
        }
    }

    TileMap<ITile> map = new TileMap<ITile>();

    void Awake()
    {
        map.Add(new Tile("Fifth", 5, 5, 3, 5)); // Fifth because it has the same elevation as fourth, but it's sorting order is higher
        map.Add(new Tile("Sixth", 5, 5, 9, -100 )); // Sixth because it has the highest elevation
        map.Add(new Tile("Third", 5, 5, 2, 9)); // Third because it has the same elevation as second, but it's sorting rder is higher
        map.Add(new Tile("Fourth", 5, 5, 3, -1)); // Fourth because it has a higher elevation that third, even though sorting order is lower
        map.Add(new Tile("Second", 5, 5, 2, 8 )); // Second because it has the second lowest elevation
        map.Add(new Tile("First", 5, 5, 1, 99999 )); // First because it has the lowest elevation

        var stack = map.GetTileStack(5, 5);
        Debug.Log("Should be all tiles in order");
        foreach( var tile in stack )
        {
            Debug.Log(tile);
        }

        Debug.Log("Should be second, third");
        foreach (var tile in map.GetTilesAtElevation(5, 5, 2))
        {
            Debug.Log(tile);
        }

        Debug.Log("Should be fourth, fifth");
        foreach (var tile in map.GetTilesAtElevation(5, 5, 3))
        {
            Debug.Log(tile);
        }
    }
}
