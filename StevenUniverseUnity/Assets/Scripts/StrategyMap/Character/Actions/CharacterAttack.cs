using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StevenUniverse.FanGame.StrategyMap
{
    // TODO: Adjust grid GetObjects so we can pass in a predicate to find specific objects...
    //       in this case the predicate will be something like actor_.faction_.IsEnemy( adjacentCharacter.faction_ )

    /// <summary>
    /// Character attack action
    /// </summary>
    public class CharacterAttack : CharacterAction
    {
        static Predicate<MapCharacter> opponentPredicate_ = null;

        void Awake()
        {
            if( opponentPredicate_ == null )
            {
                //opponentPredicate_ = (c)=>c.sta
            }
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

                grid.GetObjects(adj, opponents_);
            }

            return opponents_.Count > 0;
        }


    }

}