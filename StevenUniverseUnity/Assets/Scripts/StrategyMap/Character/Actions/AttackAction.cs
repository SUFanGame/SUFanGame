using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.Factions;
using StevenUniverse.FanGame.Util.Logic.States;

namespace StevenUniverse.FanGame.StrategyMap
{
    /// <summary>
    /// Character attack action
    /// </summary>
    public class AttackAction : CharacterAction
    {
        static Predicate<MapCharacter> opponentPredicate_ = null;

        // TODO: Attack properties should be retrieved from whatever weapon the character is currently using.
        #region AttackProperties
        public TargetType targetType_ = TargetType.ENEMY;
        public int range_ = 1;
        #endregion


        /// <summary>
        /// Target to be attacked
        /// </summary>
        //public MapCharacter Target { get; set; }

        protected override void Awake()
        {
            base.Awake();

            // Set our predicate to search only for enemies
            opponentPredicate_ = (c) =>
            {
                return actor_.Data.Faction_.GetStanding(c.Data.Faction_) == Standing.HOSTILE;
            };
        }

        List<MapCharacter> opponents_ = new List<MapCharacter>();

        public override bool IsUsable()
        {
            var grid = Grid.Instance;

            var pos = actor_.GridPosition;

            var adjacent = Directions2D.Quadrilateral;

            opponents_.Clear();
            for ( int i = 0; i < adjacent.Length; ++i )
            {
                var adj = pos + adjacent[i];

                grid.GetObjects(adj, opponents_, opponentPredicate_ );
            }

            return opponents_.Count > 0;
        }

        public IEnumerator Execute( MapCharacter target )
        {
            // Play cool attack scene animation
            //return base.Routine();
            yield return null;
        }

        public override State GetUIState()
        {
            return new ChooseTargetState(actor_, targetType_);
        }
    }

}