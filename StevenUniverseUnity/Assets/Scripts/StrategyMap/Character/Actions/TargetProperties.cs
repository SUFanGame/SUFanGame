using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Factions;

/// <summary>
/// Defines the area and or units that an action targets
/// </summary>
[System.Serializable]
public struct TargetProperties
{
    /// <summary>
    /// Target type of the action, determines which units/tiles can be targeted.
    /// </summary>
    // Hidden in inspector for now until I make a proper interface for bit enums.
    [HideInInspector]
    public TargetType type_;
    /// <summary>
    /// The range of the action, where 1 is a single tile, and each subsequent point of range adds to the area
    /// in a manhattan distance style:
    ///    1: X
    ///    ---------
    ///        X
    ///    2: XXX
    ///        X
    ///     -------
    ///          X
    ///         XXX
    ///    3:  XXXXX
    ///         XXX
    ///          X
    /// </summary>
    public int range_;
    /// <summary>
    /// The source of the action.
    /// </summary>
    [HideInInspector]
    public MapCharacter source_;

    public TargetProperties( TargetType type, int range, MapCharacter source )
    {
        type_ = type;
        range_ = range;
        source_ = source;
    }

    public bool IsValid( MapCharacter target )
    {
        //Debug.LogFormat("Testing against {0}", target.name);
        //Debug.LogFormat("Source null: {0}, Target null: {0}", source_ == null, target == null);

        var sourceFaction = source_.OwningPlayer.Faction_;
        var tarFaction = target.OwningPlayer.Faction_;


        // Check if we're targeting enemies
        if( (type_ & TargetType.ENEMY) == TargetType.ENEMY )
        {
            //Debug.LogFormat("We're targeting enemies");
            if (sourceFaction.GetStanding(tarFaction) == Standing.HOSTILE)
            {
                //Debug.LogFormat("{0} and {1} are enemies", source_.name, target.name);
                return true;
            }
        }

        // Check if we're targeting allies
        if( (type_ & TargetType.ALLY) == TargetType.ALLY )
        {
            if( sourceFaction.GetStanding(tarFaction) == Standing.FRIENDLY )
            {
                return true;
            }
        }

        // Check if we're targeting ourselves.
        if( (type_ & TargetType.SELF) == TargetType.SELF)
        {
            if (source_ == target)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Given a position on the map this will populate the given buffer with all valid targets from that position.
    /// </summary>
    /// <param name="pos">The position on the map to search from.</param>
    /// <param name="buffer">A buffer that will be populated with valid targets.</param>
    public void GetTargets( IntVector2 pos, List<MapCharacter> buffer )
    {
        
    }

    // vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv For if abilities get more complicated.
    //public int area_;
    
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
