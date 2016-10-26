using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Factions;

namespace StevenUniverse.FanGame.Factions
{
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
    /// Struct to map other factions to their standings with a faction.
    /// </summary>
    struct StandingData
    {
        public Faction faction_;
        public Dictionary<Faction, Standing> standings_;
    }

    /// <summary>
    /// Dictionary mapping each faction to it's standing data.
    /// </summary>
    static Dictionary<Faction, StandingData> factionData_ = new Dictionary<Faction, StandingData>();

    static FactionExtensions()
    {
        Debug.LogFormat("Setting up factions");
        var values = System.Enum.GetValues(typeof(Faction));
        foreach( Faction faction in values )
        {
            //Debug.LogFormat("Adding {0} to factionData", v.ToString());
            var data = new StandingData() { faction_ = faction, standings_ = new Dictionary<Faction, Standing>() };
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
        return factionData_[a].standings_[b];
    }

    /// <summary>
    /// Set the standing between two factions.
    /// </summary>
    public static void SetStanding( this Faction a, Faction b, Standing standing )
    {
        factionData_[a].standings_[b] = standing;
        factionData_[b].standings_[a] = standing;
    }
}


