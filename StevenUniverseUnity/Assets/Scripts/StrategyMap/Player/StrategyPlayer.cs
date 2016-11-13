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
        protected List<MapCharacter> units_ = new List<MapCharacter>();
        public IList<MapCharacter> Units { get; private set; }

        bool currentlyActing_ = false;

        public bool CurrentlyActing_ { get { return currentlyActing_; } }

        /// <summary>
        /// Called when this player's turn starts.
        /// </summary>
        public virtual void OnTurnStart()
        {
            currentlyActing_ = true;


        }

        public virtual void OnTurnEnd()
        {
            currentlyActing_ = false;

            for (int i = 0; i < units_.Count; ++i)
            {
                units_[i].Paused_ = false;
            }
        }

        
        /// <summary>
        /// Coroutine where the player can perform logic. 
        /// For AI players they can scan the grid and make decisions.
        /// The player's turn will be over when this function returns.
        /// </summary>
        public virtual IEnumerator Tick()
        {
            while (true)
            {

                bool allActed = true;

                for (int i = 0; i < units_.Count; ++i)
                {
                    if (units_[i].Paused_ == false)
                        allActed = false;
                }

                if (allActed)
                    break;

                yield return null;
            }
        }


        void Awake()
        {
            Units = units_.AsReadOnly();
        }   
    }
}