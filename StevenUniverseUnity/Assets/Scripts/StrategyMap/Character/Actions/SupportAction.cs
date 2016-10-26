using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Interactions;

// TODO : Push character scanning stuff out from attack/support into a utility class or the grid class.
namespace StevenUniverse.FanGame.StrategyMap
{
    public class SupportAction : CharacterAction
    {
        System.Predicate<MapCharacter> allyPredicate_;

        List<MapCharacter> allies_ = new List<MapCharacter>();

        // Set through the inspector
        public Support support_;

        protected override void Awake()
        {
            base.Awake();

            allyPredicate_ = (other) =>
            {
                return actor_.Data.Faction_.GetStanding(other.Data.Faction_) == Factions.Standing.FRIENDLY 
                //&& actor_.Data.SupportInfos.Contains( other )
                ;
            };
            
            support_.Dialog = SupportLoader.ImportSupport("Righty_Lefty_C");
        }

        public override void Execute()
        {
            support_.gameObject.SetActive(true);
        }

        public override bool IsUsable()
        {
            var grid = Grid.Instance;

            var pos = actor_.GridPosition;

            var adjacent = Directions2D.Quadrilateral;

            allies_.Clear();
            for (int i = 0; i < adjacent.Length; ++i)
            {
                var adj = pos + adjacent[i];

                grid.GetObjects(adj, allies_, allyPredicate_);
            }

            return allies_.Count > 0;
        }
    }
}
