using System.Collections.Generic;
using StevenUniverse.FanGame.Interactions;
using System.Collections;

// TODO : Push character scanning stuff out from attack/support into a utility class or the grid class.
namespace StevenUniverse.FanGame.StrategyMap
{
    public class SupportAction : CharacterAction
    {
        System.Predicate<MapCharacter> allyPredicate_;

        List<MapCharacter> allies_ = new List<MapCharacter>();

        // Set through the inspector
        public Support supportCanvas_;

        protected override void Awake()
        {
            base.Awake();

            allyPredicate_ = (other) =>
            {
                return actor_.Data.Faction_.GetStanding(other.Data.Faction_) == Factions.Standing.FRIENDLY 
                //&& actor_.Data.SupportInfos.Contains( other )
                ;
            };
            
        }

        public override IEnumerator Routine()
        {
            supportCanvas_.Dialog = SupportLoader.ImportSupport("Righty_Lefty_C");
            supportCanvas_.gameObject.SetActive(true);

            yield return base.Routine();
        }

        
        public override bool IsUsable()
        {
            // First check for adjacent allies
            var grid = Grid.Instance;
            var pos = actor_.GridPosition;
            var adjacent = Directions2D.Quadrilateral;
            allies_.Clear();

            for (int i = 0; i < adjacent.Length; ++i)
            {
                var adj = pos + adjacent[i];

                grid.GetObjects(adj, allies_, allyPredicate_);
            }

            // Check if allies can talk
            //foreach (MapCharacter ally in allies_)
            //{

            //}

            return allies_.Count > 0;
        }
    }
}
