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
        }

        

        /// <summary>
        /// Coroutine where the player can perform logic. For a human player this will be
        /// things like UI handling. For AI players they can scan the grid and make decisions.
        /// The player's turn will be over when this function returns.
        /// </summary>
        public virtual IEnumerator Tick()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.T));
        }


        void Awake()
        {

        }   
    }
}