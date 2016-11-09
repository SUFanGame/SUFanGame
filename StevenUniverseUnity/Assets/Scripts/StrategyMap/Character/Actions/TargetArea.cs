using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;

/// <summary>
/// Defines the area that an action targets
/// </summary>
public struct TargetArea
{
    public TargetType type_;
    public int range_;
    
    //List<IntVector2> points_ = new List<IntVector2>();

    // TODO: Allow points to be defined in JSON, Maybe something like:

    // @ = Cursor, X = targets, * = Cursor + target?
    //
    //   X
    //@ X X
    //   X
    // Parse until @ or * is found, use that as origin, parse targets, subtract origin from all targets and you have
    // the points relative to cursor. That defines the area of effect, range can be it's own property and defines how far the 
    // cursor can be placed from the character.
    

    // With the option to define "standard" areas like FFT or Shining Force.

    // This should be pretty simple to set up with FullSerializer
}
