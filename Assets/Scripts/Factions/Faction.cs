﻿using System.Collections.Generic;
using SUGame.Factions;
using UnityEngine;

namespace SUGame.Factions
{
    /// <summary>
    /// Each faction has a standing which can be polled via Faction.GetStanding
    /// or set via Faction.SetStanding.
    /// </summary>
    public enum Faction
    {
        CRYSTALGEMS,
        HOMEWORLD,
        YOURMOM
    }

    public enum Standing
    {
        HOSTILE,
        FRIENDLY,
        NEUTRAL
    }
}

public static class FactionExtensions
{
    /// <summary>
    /// Data to map other factions to their standings with a faction.
    /// </summary>
    class StandingData
    {
        public Faction faction_;
        public Dictionary<Faction, Standing> standings_ = new Dictionary<Faction, Standing>();
    }

    /// <summary>
    /// Dictionary mapping each faction to it's standing data.
    /// </summary>
    static Dictionary<Faction, StandingData> factionData_ = new Dictionary<Faction, StandingData>();

    static FactionExtensions()
    {
        //Debug.LogFormat("Setting up factions");
        var values = System.Enum.GetValues(typeof(Faction));
        foreach( Faction faction in values )
        {
            //Debug.LogFormat("Adding {0} to factionData", v.ToString());
            var data = new StandingData() { faction_ = faction };
            factionData_.Add( faction, data );

            // Ensure checking a faction against itself returns friendly
            SetStanding(faction, faction, Standing.FRIENDLY);
        }

        // Set up initial standing values
        SetStanding(Faction.CRYSTALGEMS, Faction.HOMEWORLD, Standing.HOSTILE);

    }

    /// <summary>
    /// Get the standing between two factions.
    /// </summary>
    public static Standing GetStanding(this Faction a, Faction b)
    {
        var standing = factionData_[a].standings_[b];
        //Debug.LogFormat("Checking {0} against {1}. Standing: {2}", a, b, standing);
        return standing;
    }

    /// <summary>
    /// Get an array of factions that have a given standing with the selected faction.
    /// </summary>
    public static Faction[] GetFactionsWithStanding(this Faction a, Standing targetStanding)
    {
        List<Faction> factionsWithStanding = new List<Faction>();

        StandingData myStandingData = factionData_[a];

        foreach (Faction otherFaction in myStandingData.standings_.Keys)
        {
            Standing otherStanding = myStandingData.standings_[otherFaction];
            if (otherStanding == targetStanding)
            {
                factionsWithStanding.Add(otherFaction);
            }
        }

        return factionsWithStanding.ToArray();
    }

    /// <summary>
    /// Set the standing between two factions.
    /// </summary>
    public static void SetStanding( this Faction a, Faction b, Standing standing )
    {
        //Debug.LogFormat("{0} and {1} are now {2}", a, b, standing);
        factionData_[a].standings_[b] = standing;
        factionData_[b].standings_[a] = standing;
    }

    ///// <summary>
    ///// Returns the target type of one faction relative to the other( ENEMY, ALLY, or NEUTRAL )
    ///// </summary>
    //public static TargetType GetTargetType( Faction a, Faction b )
    //{
    //    switch( GetStanding(a, b) )
    //    {
    //        case Standing.FRIENDLY:
    //            return TargetType.ALLY;
    //        case Standing.HOSTILE:
    //            return TargetType.ENEMY;
    //        default:
    //            return TargetType.
    //    }
    //}
}


