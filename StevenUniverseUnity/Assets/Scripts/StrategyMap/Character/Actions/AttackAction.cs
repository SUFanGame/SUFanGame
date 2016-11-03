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

        List<MapCharacter> opponents_ = new List<MapCharacter>();

        protected override void Awake()
        {
            base.Awake();

            // Set our predicate to search only for enemies
            opponentPredicate_ = (c) =>
            {
                return actor_.Data.Faction_.GetStanding(c.Data.Faction_) == Standing.HOSTILE;
            };
        }


        public override bool IsUsable()
        {
            return CharacterUtility.ScanForAdjacent(actor_, opponentPredicate_).Count > 0;
        }


    }

}