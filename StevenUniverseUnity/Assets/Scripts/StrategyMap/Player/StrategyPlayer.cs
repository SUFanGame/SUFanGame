using StevenUniverse.FanGame.Factions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StevenUniverse.FanGame.StrategyMap.Players
{
    /// <summary>
    /// Represents a player controlling a set of units on a strategy map.
    /// Could be a human or an AI player
    /// </summary>
    public class StrategyPlayer : MonoBehaviour
    {
        [SerializeField]
        Faction faction_;

        public Faction Faction_ { get { return faction_; } }

        [SerializeField]
        List<MapCharacter> units_ = new List<MapCharacter>();

        /// <summary>
        /// Called when this player's turn starts.
        /// </summary>
        protected virtual void OnTurnStart()
        {
            
        }
        
        void Awake()
        {

        }   
    }
}