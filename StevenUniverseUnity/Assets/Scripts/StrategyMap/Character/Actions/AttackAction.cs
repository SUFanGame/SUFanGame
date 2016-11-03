using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StevenUniverse.FanGame.Factions;

namespace StevenUniverse.FanGame.StrategyMap
{
    // TODO: Adjust grid GetObjects so we can pass in a predicate to find specific objects...
    //       in this case the predicate will be something like actor_.faction_.IsEnemy( adjacentCharacter.faction_ )

    /// <summary>
    /// Character attack action
    /// </summary>
    public class AttackAction : CharacterAction
    {
        static Predicate<MapCharacter> opponentPredicate_ = null;

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
        
    }

}